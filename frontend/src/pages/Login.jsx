import {
  Box,
  Button,
  Typography,
  TextField,
  Paper,
  InputAdornment,
  IconButton,
  CircularProgress,
  Alert
} from "@mui/material";
import { Email, Lock, Visibility, VisibilityOff } from "@mui/icons-material";
import { useState } from "react";
import { useForm } from "react-hook-form";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../auth/useAuth";
import { useLoginApi } from "../hooks/useLogin";

const Login = () => {
  const { register, handleSubmit, formState: { errors } } = useForm();
  const [showPassword, setShowPassword] = useState(false);
  const navigate = useNavigate();

  const { login, loading, error } = useAuth(); // 🔹 error state'i aldık
  const { loginFunc } = useLoginApi();

  const onSubmit = async (data) => {
    const result = await login(data.email, data.password, loginFunc);
    if (result.success) {
      navigate("/home");
    }
    // 🔹 error state zaten AuthContext tarafından güncelleniyor, ek bir setState gerek yok
  };

  return (
    <Box className="login-page">
      <Paper className="login-card">
        <Typography variant="h4" className="login-title">
          Welcome!
        </Typography>
        <Typography className="login-subtitle">
          Sign in to continue to your app
        </Typography>

        {/* 🔹 Backend veya AuthContext tarafından gelen hata mesajı */}
        {error && <Alert severity="error" className="login-error">{error}</Alert>}

        <form onSubmit={handleSubmit(onSubmit)}>
          <TextField
            label="Email"
            fullWidth
            margin="normal"
            {...register("email", { required: "Email is required" })}
            error={!!errors.email}
            helperText={errors.email?.message}
            InputLabelProps={{ className: "input-label" }}
            InputProps={{
              startAdornment: (
                <InputAdornment position="start">
                  <Email className="input-icon" />
                </InputAdornment>
              ),
            }}
          />

          <TextField
            label="Password"
            type={showPassword ? "text" : "password"}
            fullWidth
            margin="normal"
            {...register("password", { required: "Password is required" })}
            error={!!errors.password}
            helperText={errors.password?.message}
            InputLabelProps={{ className: "input-label" }}
            InputProps={{
              startAdornment: (
                <InputAdornment position="start">
                  <Lock className="input-icon" />
                </InputAdornment>
              ),
              endAdornment: (
                <InputAdornment position="end">
                  <IconButton onClick={() => setShowPassword(!showPassword)}>
                    {showPassword ? (
                      <VisibilityOff className="input-icon" />
                    ) : (
                      <Visibility className="input-icon" />
                    )}
                  </IconButton>
                </InputAdornment>
              ),
            }}
          />

          <Button
            type="submit"
            fullWidth
            className="login-btn"
            disabled={loading}
          >
            {loading ? <CircularProgress size={24} color="inherit" /> : "Login"}
          </Button>
        </form>
      </Paper>
    </Box>
  );
};

export default Login;
