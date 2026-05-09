# Feature Specification: User Login

**Feature Branch**: `001-user-login`
**Created**: 2026-05-08
**Status**: Draft
**Input**: User description: "Login con usuario y contraseña, usando APIs y front de React"

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Login Exitoso (Priority: P1)

Un usuario registrado ingresa su email y contraseña en el formulario de login y accede al sistema exitosamente.

**Why this priority**: Es el flujo principal y más crítico — sin él la aplicación no es funcional.

**Independent Test**: Se puede probar ingresando credenciales válidas y verificando que el usuario llega a la pantalla principal autenticado.

**Acceptance Scenarios**:

1. **Given** un usuario registrado con credenciales válidas, **When** ingresa email y contraseña y presiona "Iniciar sesión", **Then** el sistema lo autentica y redirige a la pantalla principal.
2. **Given** un usuario autenticado, **When** recarga la página, **Then** su sesión se mantiene activa sin necesidad de volver a ingresar credenciales.
3. **Given** un usuario autenticado, **When** cierra sesión, **Then** es redirigido al login y no puede acceder a rutas protegidas.

---

### User Story 2 - Credenciales Incorrectas (Priority: P2)

Un usuario intenta iniciar sesión con credenciales inválidas y recibe un mensaje de error claro sin comprometer la seguridad.

**Why this priority**: Proteger el sistema y guiar al usuario ante errores es fundamental para seguridad y usabilidad.

**Independent Test**: Se puede probar ingresando credenciales incorrectas y verificando que aparece un mensaje de error genérico sin revelar cuál campo falló.

**Acceptance Scenarios**:

1. **Given** un usuario con contraseña incorrecta, **When** intenta iniciar sesión, **Then** ve un mensaje de error genérico sin indicar cuál campo falló.
2. **Given** un email no registrado, **When** intenta iniciar sesión, **Then** ve el mismo mensaje de error genérico.
3. **Given** 5 intentos fallidos consecutivos, **When** intenta iniciar sesión nuevamente, **Then** la cuenta es bloqueada temporalmente y se muestra un mensaje informativo.

---

### User Story 3 - Validación de Formulario (Priority: P3)

El formulario valida los campos antes de enviar la solicitud al servidor.

**Why this priority**: Mejora la experiencia del usuario evitando llamadas innecesarias al servidor.

**Independent Test**: Se puede probar enviando el formulario vacío o con email mal formateado y verificando mensajes de validación locales.

**Acceptance Scenarios**:

1. **Given** el formulario vacío, **When** el usuario presiona "Iniciar sesión", **Then** ambos campos muestran mensajes de campo requerido.
2. **Given** un email con formato inválido, **When** el usuario presiona "Iniciar sesión", **Then** el campo email muestra un mensaje de formato inválido.

---

### Edge Cases

- Si el servidor no responde, el usuario ve un mensaje de error de conectividad sin perder los datos ingresados.
- Si el usuario tiene sesión activa y navega al login, es redirigido automáticamente a la pantalla principal.
- Si el token de sesión expira mientras el usuario está activo, se le solicita iniciar sesión nuevamente preservando la URL de destino.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: El sistema MUST permitir a usuarios autenticarse mediante email y contraseña.
- **FR-002**: El sistema MUST validar el formato del email en el cliente antes de enviar la solicitud.
- **FR-003**: El sistema MUST responder con un mensaje de error genérico ante credenciales inválidas, sin revelar cuál campo es incorrecto.
- **FR-004**: El sistema MUST bloquear temporalmente una cuenta tras 5 intentos fallidos consecutivos.
- **FR-005**: El sistema MUST mantener la sesión activa entre recargas de página.
- **FR-006**: El sistema MUST permitir al usuario cerrar sesión explícitamente.
- **FR-007**: El sistema MUST redirigir al usuario autenticado que navega al login hacia la pantalla principal.
- **FR-008**: El sistema MUST redirigir al usuario no autenticado que accede a rutas protegidas hacia el login.
- **FR-009**: El sistema MUST preservar la URL de destino al redirigir al login, para restaurarla tras autenticación exitosa.

### Key Entities

- **Usuario**: Entidad con email único, contraseña almacenada de forma segura, estado de cuenta (activo/bloqueado) y contador de intentos fallidos.
- **Sesión**: Representación del estado autenticado del usuario con tiempo de expiración configurable.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Un usuario puede completar el proceso de login exitoso en menos de 30 segundos.
- **SC-002**: El sistema responde a una solicitud de login en menos de 2 segundos bajo carga normal.
- **SC-003**: El 95% de usuarios completa el login exitosamente en el primer intento con credenciales válidas.
- **SC-004**: El formulario muestra mensajes de validación antes del envío, eliminando llamadas innecesarias por campos vacíos o mal formateados.
- **SC-005**: Ningún mensaje de error revela información sobre la existencia de un email en el sistema.

## Assumptions

- Los usuarios ya están registrados en el sistema (registro fuera del alcance de esta feature).
- El sistema de almacenamiento de usuarios existe o será provisto por otra feature.
- La sesión se maneja mediante tokens seguros gestionados por el backend.
- El bloqueo temporal de cuenta dura 15 minutos (estándar de la industria).
- El tiempo de expiración de sesión inactiva es de 8 horas (jornada laboral estándar).
- La feature cubre únicamente autenticación por email/contraseña; SSO y OAuth están fuera de alcance para v1.
