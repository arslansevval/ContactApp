import { useState } from "react";
import { Link } from "react-router-dom";

const Navbar = () => {
  const [open, setOpen] = useState(false);

  return (
    <nav className="navbar">
      <div className="navbar-container">

        {/* Logo */}
        <div className="navbar-logo">
          <Link to="/home">ContactApp</Link>
        </div>

        {/* Hamburger */}
        <div className="navbar-toggle" onClick={() => setOpen(!open)}>
          <span className={open ? "bar rotate1" : "bar"}></span>
          <span className={open ? "bar hide" : "bar"}></span>
          <span className={open ? "bar rotate2" : "bar"}></span>
        </div>

        {/* Menü */}
        <ul className={`navbar-menu ${open ? "active" : ""}`}>
          <li><Link to="/home" onClick={() => setOpen(false)}>Home</Link></li>
          <li><Link to="/employees" onClick={() => setOpen(false)}>Employees</Link></li>
          <li><Link to="/companies" onClick={() => setOpen(false)}>Companies</Link></li>
          <li><Link to="/login" onClick={() => setOpen(false)}>Logout</Link></li>
        </ul>
      </div>
    </nav>
  );
};

export default Navbar;
