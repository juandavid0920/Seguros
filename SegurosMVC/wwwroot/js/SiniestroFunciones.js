async function gestionarSiniestro() {
    // Obtiene los datos del formulario
    const polizaId = document.getElementById('polizaId').value;
    const fechaSiniestro = document.getElementById('fechaSiniestro').value;
    const descripcion = document.getElementById('detalleSiniestro').value;
    const montoReclamo = document.getElementById('valorReclamar').value;

    if (!polizaId || !fechaSiniestro || !descripcion || !montoReclamo) {
        alert("Por favor, complete todos los campos del formulario.");
        return;
    }

    // Crea el objeto para enviar
    const siniestro = {
        PolizaId: parseInt(polizaId),
        FechaSiniestro: fechaSiniestro,
        Descripcion: descripcion,
        MontoReclamo: parseFloat(montoReclamo),
        Estado: "Pendiente"
    };

    try {
        // Envía los datos al servidor usando fetch
        const response = await fetch('/Gestiones/Siniestro_AgregarNuevoPoliza', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(siniestro)
        });

        const result = await response.json();

        if (result.success) {
            alert(result.message);
            // Llama a la función para recargar la tabla de siniestros
            cargarSiniestrosPorPoliza(polizaId);
        } else {
            alert(result.message);
        }
    } catch (error) {
        console.error("Error al gestionar el siniestro:", error);
        alert("Ocurrió un error al gestionar el siniestro. Inténtelo de nuevo más tarde.");
    }
}

async function cargarSiniestrosPorPoliza(polizaId) {
    try {
        // Llamada al controlador Gestiones para obtener los siniestros
        const response = await fetch(`/Gestiones/Siniestro_ConsultaPoliza?polizaId=${polizaId}`);
        const data = await response.json();

        if (data.success) {
            // Obtenemos la tabla de siniestros
            const tablaSiniestros = document.querySelector("#tablaSiniestros tbody");

            // Limpiamos cualquier contenido previo en la tabla
            tablaSiniestros.innerHTML = "";

            // Verificamos si hay datos de siniestros
            if (data.data && data.data.length > 0) {
                data.data.forEach(siniestro => {
                    // Agregamos cada siniestro a la tabla
                    const fila = `
                        <tr>
                            <td>${new Date(siniestro.fechaSiniestro).toLocaleDateString()}</td>
                            <td>${siniestro.descripcion}</td>
                            <td>$ ${siniestro.montoReclamo.toFixed(2)}</td>
                        </tr>
                    `;
                    tablaSiniestros.innerHTML += fila;
                });
            } else {
                // Si no hay datos, mostramos un mensaje vacío
                const filaVacia = `
                    <tr>
                        <td colspan="3" class="text-center">No hay siniestros registrados para esta póliza.</td>
                    </tr>
                `;
                tablaSiniestros.innerHTML += filaVacia;
            }

            // Mostramos la tabla de siniestros
            document.getElementById("tablaSiniestros").style.display = "block";
        } else {
            alert(data.message || "Error al cargar los siniestros.");
        }
    } catch (error) {
        console.error("Error al cargar los siniestros:", error);
        alert("Ocurrió un error al cargar los siniestros. Inténtelo más tarde.");
    }
}

function rellenarFormulario(polizaId, tipoPoliza) {
    document.getElementById('polizaId').value = polizaId;
    document.getElementById('tipoPoliza').value = tipoPoliza;
    document.getElementById('formularioSiniestro').style.display = 'block';
    document.getElementById('tablaSiniestros').style.display = 'block';

    // Cargar los siniestros de la póliza seleccionada
    cargarSiniestrosPorPoliza(polizaId);
}
