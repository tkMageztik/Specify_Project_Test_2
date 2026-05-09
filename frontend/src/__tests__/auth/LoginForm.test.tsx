import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, expect, it, vi } from 'vitest';
import { LoginForm } from '../../features/auth/LoginForm';

describe('LoginForm', () => {
  it('calls onSubmit with email and password on valid submit', async () => {
    const onSubmit = vi.fn().mockResolvedValue(undefined);
    render(<LoginForm onSubmit={onSubmit} />);

    await userEvent.type(screen.getByLabelText(/email/i), 'user@example.com');
    await userEvent.type(screen.getByLabelText(/password/i), 'password123');
    await userEvent.click(screen.getByRole('button', { name: /sign in/i }));

    await waitFor(() => {
      expect(onSubmit).toHaveBeenCalledWith('user@example.com', 'password123');
    });
  });

  it('displays server error message when error prop is provided', () => {
    render(<LoginForm onSubmit={vi.fn()} error="Invalid email or password." />);

    expect(screen.getByRole('alert')).toHaveTextContent('Invalid email or password.');
  });

  it('shows required field errors when submitting empty form', async () => {
    render(<LoginForm onSubmit={vi.fn()} />);

    await userEvent.click(screen.getByRole('button', { name: /sign in/i }));

    await waitFor(() => {
      expect(screen.getByText('Email is required.')).toBeInTheDocument();
      expect(screen.getByText('Password is required.')).toBeInTheDocument();
    });
  });

  it('shows email format error for invalid email', async () => {
    render(<LoginForm onSubmit={vi.fn()} />);

    await userEvent.type(screen.getByLabelText(/email/i), 'notanemail');
    await userEvent.click(screen.getByRole('button', { name: /sign in/i }));

    await waitFor(() => {
      expect(screen.getByText('Invalid email format.')).toBeInTheDocument();
    });
  });
});
