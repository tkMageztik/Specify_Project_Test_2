import { useState } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import axios from 'axios';
import { LoginForm } from './LoginForm';
import { useAuth } from './useAuth';

export function LoginPage() {
  const { login } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const [error, setError] = useState<string>();
  const redirectTo = (location.state as { from?: { pathname?: string } } | null)?.from?.pathname ?? '/';

  const handleLogin = async (email: string, password: string) => {
    try {
      setError(undefined);
      await login(email, password);
      navigate(redirectTo, { replace: true });
    } catch (err) {
      if (axios.isAxiosError(err)) {
        const status = err.response?.status;
        const message = err.response?.data?.error;
        if (status === 401 || status === 423) {
          setError(message ?? 'Invalid email or password.');
          return;
        }
      }

      setError('An unexpected error occurred. Please try again.');
    }
  };

  return (
    <main>
      <h1>Sign in</h1>
      <LoginForm onSubmit={handleLogin} error={error} />
    </main>
  );
}
