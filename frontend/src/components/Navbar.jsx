import { useState, useContext } from "react";
import { Link } from "react-router-dom";
import { AuthContext } from "../auth/AuthContext.jsx";

const Navbar = () => {
  const [open, setOpen] = useState(false);
  const { logout, isAuthenticated } = useContext(AuthContext);

  if (!isAuthenticated) return null; // login değilse navbar gösterme

  const handleLogout = () => {
    logout();       // token ve user sıfırlanır
    setOpen(false); // hamburger menüyü kapat
  };

  return (
    <nav className="navbar">
      <div className="navbar-container">
        <div className="navbar-logo">
          <Link to="/home">ContactApp</Link>
        </div>

        <div className="navbar-toggle" onClick={() => setOpen(!open)}>
          <span className={open ? "bar rotate1" : "bar"}></span>
          <span className={open ? "bar hide" : "bar"}></span>
          <span className={open ? "bar rotate2" : "bar"}></span>
        </div>

        <ul className={`navbar-menu ${open ? "active" : ""}`}>
          <li><Link to="/home" onClick={() => setOpen(false)}>Home</Link></li>
          <li><Link to="/employees" onClick={() => setOpen(false)}>Employees</Link></li>
          <li><Link to="/companies" onClick={() => setOpen(false)}>Companies</Link></li>
          <li><Link  onClick={handleLogout}>Logout</Link></li>
        </ul>
      </div>
    </nav>
  );
};

export default Navbar;
