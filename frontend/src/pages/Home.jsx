const Home = () => {
  return (
    <div className="home-container">
      <div className="home-card">
        <h1 className="home-title">Welcome Back!</h1>
        <p className="home-subtitle">Here’s your dashboard overview</p>

        <div className="home-grid">
          <div className="home-box" >Employees</div>
          <div className="home-box">Companies</div>
          <div className="home-box">Reports</div>
          <div className="home-box">Settings</div>
        </div>
      </div>
    </div>
  );
};

export default Home;
