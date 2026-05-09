import { render, screen } from '@testing-library/react';
import { MemoryRouter, Route, Routes, useLocation } from 'react-router-dom';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import { ProtectedRoute } from '../../features/auth/ProtectedRoute';
import { useAuth } from '../../features/auth/useAuth';

vi.mock('../../features/auth/useAuth', () => ({
  useAuth: vi.fn(),
}));

const mockedUseAuth = vi.mocked(useAuth);

function LoginStateProbe() {
  const location = useLocation();
  return <div>{(location.state as { from?: { pathname?: string } } | null)?.from?.pathname ?? 'missing'}</div>;
}

describe('ProtectedRoute', () => {
  beforeEach(() => {
    mockedUseAuth.mockReset();
  });

  it('renders a loading indicator while auth state is loading', () => {
    mockedUseAuth.mockReturnValue({
      accessToken: null,
      isAuthenticated: false,
      isLoading: true,
      login: vi.fn(),
      logout: vi.fn(),
      refresh: vi.fn(),
    });

    render(
      <MemoryRouter initialEntries={['/private']}>
        <Routes>
          <Route element={<ProtectedRoute />}>
            <Route path="/private" element={<div>Private</div>} />
          </Route>
        </Routes>
      </MemoryRouter>,
    );

    expect(screen.getByText('Loading…')).toBeInTheDocument();
  });

  it('renders the protected content when authenticated', () => {
    mockedUseAuth.mockReturnValue({
      accessToken: 'token',
      isAuthenticated: true,
      isLoading: false,
      login: vi.fn(),
      logout: vi.fn(),
      refresh: vi.fn(),
    });

    render(
      <MemoryRouter initialEntries={['/private']}>
        <Routes>
          <Route element={<ProtectedRoute />}>
            <Route path="/private" element={<div>Private</div>} />
          </Route>
        </Routes>
      </MemoryRouter>,
    );

    expect(screen.getByText('Private')).toBeInTheDocument();
  });

  it('redirects unauthenticated users to login and preserves the original route', () => {
    mockedUseAuth.mockReturnValue({
      accessToken: null,
      isAuthenticated: false,
      isLoading: false,
      login: vi.fn(),
      logout: vi.fn(),
      refresh: vi.fn(),
    });

    render(
      <MemoryRouter initialEntries={['/private']}>
        <Routes>
          <Route path="/login" element={<LoginStateProbe />} />
          <Route element={<ProtectedRoute />}>
            <Route path="/private" element={<div>Private</div>} />
          </Route>
        </Routes>
      </MemoryRouter>,
    );

    expect(screen.getByText('/private')).toBeInTheDocument();
  });
});
