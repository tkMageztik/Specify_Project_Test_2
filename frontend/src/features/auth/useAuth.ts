import {
  createContext,
  createElement,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useState,
  type ReactNode,
} from 'react';
import { authApi, type LoginResponse } from '../../api/authApi';

interface AuthState {
  accessToken: string | null;
  isAuthenticated: boolean;
  isLoading: boolean;
}

interface AuthContextValue extends AuthState {
  login: (email: string, password: string) => Promise<LoginResponse>;
  logout: () => Promise<void>;
  refresh: () => Promise<string | null>;
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [state, setState] = useState<AuthState>({
    accessToken: null,
    isAuthenticated: false,
    isLoading: true,
  });

  const refresh = useCallback(async () => {
    try {
      const { data } = await authApi.refresh();
      setState({ accessToken: data.accessToken, isAuthenticated: true, isLoading: false });
      return data.accessToken;
    } catch {
      setState({ accessToken: null, isAuthenticated: false, isLoading: false });
      return null;
    }
  }, []);

  useEffect(() => {
    void refresh();
  }, [refresh]);

  const login = useCallback(async (email: string, password: string) => {
    const { data } = await authApi.login({ email, password });
    setState({ accessToken: data.accessToken, isAuthenticated: true, isLoading: false });
    return data;
  }, []);

  const logout = useCallback(async () => {
    await authApi.logout();
    setState({ accessToken: null, isAuthenticated: false, isLoading: false });
  }, []);

  const value = useMemo<AuthContextValue>(() => ({ ...state, login, logout, refresh }), [state, login, logout, refresh]);

  return createElement(AuthContext.Provider, { value }, children);
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider.');
  }

  return context;
}
