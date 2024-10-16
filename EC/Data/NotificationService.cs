using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;
using MimeKit;

namespace EC.Data
{
    public class NotificationService : BackgroundService
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly SmtpClient _smtp;

        public NotificationService(IServiceScopeFactory factory, IConfiguration configuration)
        {
            _context = factory.CreateScope().ServiceProvider.GetRequiredService<DataContext>();
            _configuration = configuration;
            _cancellationTokenSource = new CancellationTokenSource();
            _smtp = new SmtpClient();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (stoppingToken.IsCancellationRequested == false)
            {
                try
                {
                    var items = await _context.ItemData.Where(item => (item.NotificationTime <= DateTime.Now) && (item.SendFlg == false)).ToArrayAsync();
                    if (items.Any() == false)
                    {
                        await Task.Delay(TimeSpan.FromMinutes(Constants.MAIL_SEND_DELAY_TIME), stoppingToken);
                    }
                    var customerIDs = items.Select(item => item.CustomerID).Distinct();
                    var customers = await _context.CustomerData.Where(customer => customerIDs.Contains(customer.CustomerID)).ToArrayAsync();
                    if (customers.Any() == false)
                    {
                        await Task.Delay(TimeSpan.FromMinutes(Constants.MAIL_SEND_DELAY_TIME), stoppingToken);
                    }
                    _smtp.Connect("smtp.gmail.com", 587, false);
                    _smtp.Authenticate(_configuration["Email:Address"], _configuration["Email:Password"]);
                    foreach (var item in items)
                    {
                        item.SendFlg = true;
                        var customer = customers.FirstOrDefault(customer => customer.CustomerID == item.CustomerID);
                        
                        if (customer == null) continue;

                        await SendEmailAsync(customer.Email, item.NotificationSubject, item.NotificationBody);
                    }
                    _context.ItemData.UpdateRange(items);
                    await _context.SaveChangesAsync();
                }
                finally
                {
                    _smtp.Disconnect(true);
                    await Task.Delay(TimeSpan.FromMinutes(Constants.MAIL_SEND_DELAY_TIME), stoppingToken);
                }
            }
        }

        private async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(string.Empty, _configuration["Email:Address"]));
            email.To.Add(new MailboxAddress(string.Empty, toEmail));
            email.Subject = subject;
            email.Body = new TextPart("plain") { Text = body };
            await _smtp.SendAsync(email);
        }

        public void StopService()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}
