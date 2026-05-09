import { useForm } from 'react-hook-form';

interface FormValues {
  email: string;
  password: string;
}

interface LoginFormProps {
  onSubmit: (email: string, password: string) => Promise<void>;
  error?: string;
}

export function LoginForm({ onSubmit, error }: LoginFormProps) {
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<FormValues>();

  const submit = async (values: FormValues) => {
    await onSubmit(values.email, values.password);
  };

  return (
    <form onSubmit={handleSubmit(submit)} noValidate>
      <div>
        <label htmlFor="email">Email</label>
        <input
          id="email"
          type="email"
          aria-describedby={errors.email ? 'email-error' : undefined}
          {...register('email', {
            required: 'Email is required.',
            pattern: {
              value: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
              message: 'Invalid email format.',
            },
          })}
        />
        {errors.email && (
          <span id="email-error" role="alert">
            {errors.email.message}
          </span>
        )}
      </div>

      <div>
        <label htmlFor="password">Password</label>
        <input
          id="password"
          type="password"
          aria-describedby={errors.password ? 'password-error' : undefined}
          {...register('password', { required: 'Password is required.' })}
        />
        {errors.password && (
          <span id="password-error" role="alert">
            {errors.password.message}
          </span>
        )}
      </div>

      {error && <div role="alert">{error}</div>}

      <button type="submit" disabled={isSubmitting}>
        {isSubmitting ? 'Signing in…' : 'Sign in'}
      </button>
    </form>
  );
}
