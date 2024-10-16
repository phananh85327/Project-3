import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import View from './pages/View'
import Main from './pages/Main'
import Login from './pages/Login'
import Update from './pages/Update'
import Error from './pages/Error'
import FetchData from './api/FetchData'

function App() {
  let useView = true;
  if ((sessionStorage.getItem(FetchData.accessToken) !== null)
    && (sessionStorage.getItem(FetchData.refreshToken) !== null)
    && (sessionStorage.getItem(FetchData.loginUser) !== null)) {
      useView = false;
  }

  return (
    <Router>
      <Routes>
        <Route path="/" element={useView ? <Navigate to="/view" /> : <Navigate to="/main" />} />
        <Route path="/view" element={<View />} />
        <Route path="/main" element={<Main />} />
        <Route path="/login" element={<Login />} />
        <Route path="/update" element={<Update />} />
        <Route path="*" element={<Error />} />
      </Routes>
    </Router>
  );
}

export default App;