// Función para mostrar un formulario y ocultar el no funcional
function mostrarFormulario(formulario) {
    const formRegistro = document.getElementById("formRegistro");
    const formLogin = document.getElementById("formLogin");

    if (formulario === "registro") {
        formRegistro.style.display = "block";
        formLogin.style.display = "none";
    } else if (formulario === "login") {
        formRegistro.style.display = "none";
        formLogin.style.display = "block";
    }
}

// Función para registrar un usuario
async function registrarUsuario() {
    // Obtén los valores del formulario
    const nombre = document.getElementById("nombre").value;
    const correo = document.getElementById("correo").value;
    const contraseña = document.getElementById("contraseña").value;
    const rol = document.getElementById("rol").value;

    if (nombre == "" || correo == "" || contraseña == "" || rol=="") {
        alert("Todos los campos son obligatorios")
    }
    else {

        // Crear el objeto con la estructura de UsuariosDTO
        const usuario = {
            UsuarioId: 0, // Se envía 0 porque no se requiere en el registro inicial
            Nombre: nombre,
            CorreoElectronico: correo,
            Rol: rol,
            Contraseña: contraseña,
            Estado: "Activo" // Puedes establecer un estado predeterminado
        };

        try {
            // Realizar la solicitud POST al controlador
            const response = await fetch('/Home/Usuarios_CrearUsuarioNuevo', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json' // Indicamos que el contenido es JSON
                },
                body: JSON.stringify(usuario) // Convertimos el objeto a JSON
            });

            // Procesar la respuesta del servidor
            const data = await response.json();

            if (data.success) {
                alert(data.message); // Mostrar mensaje de éxito
                document.getElementById("formRegistroUsuario").reset(); // Limpiar formulario
            } else {
                alert(data.message); // Mostrar mensaje de error
            }
        } catch (error) {
            console.error('Error al registrar el usuario:', error);
            alert("Ocurrió un error al intentar registrar el usuario.");
        }

    }

    
}

// Función para manejar el login (puedes implementarlo más adelante)
async function iniciarSesion() {
    const correo = document.getElementById("correoLogin").value;
    const contraseña = document.getElementById("contraseñaLogin").value;


    if (correo == "" || contraseña == "") {
        alert("Digite el correo y la contraseña")
    }
    else {

        // Crear el objeto con la estructura de UsuariosDTO
        const usuario = {
            UsuarioId: 0, // Se envía 0 porque no se requiere en el registro inicial
            Nombre: '',
            CorreoElectronico: correo,
            Rol: '',
            Contraseña: contraseña,
            Estado: '' // Puedes establecer un estado predeterminado
        };

        try {
            // Realizar la solicitud POST al controlador
            const response = await fetch('/Home/LoginUsuario', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json' // Indicamos que el contenido es JSON
                },
                body: JSON.stringify(usuario) // Convertimos el objeto a JSON
            });

            // Procesar la respuesta del servidor
            const data = await response.json();

            if (data.success) {
                alert(data.message); // Mostrar mensaje de éxito
                window.location.href = data.redireccion;
            } else {
                alert(data.message); // Mostrar mensaje de error
            }
        } catch (error) {
            console.error('Error al registrar el usuario:', error);
            alert("Ocurrió un error al intentar registrar el usuario.");
        }

    }
}