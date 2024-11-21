//Metodo para consulta del costo de la poliza
document.getElementById("tipoPoliza").addEventListener("change", async function () {
    const tipoPolizaId = this.value;

    try {
        const response = await fetch('/Gestiones/ObtenerCostoPorTipoPoliza?tipoPolizaId=' + tipoPolizaId);
        const data = await response.json();

        if (data.success) {
            document.getElementById("costo").value = data.costo;
        } else {
            alert(data.message);
        }
    } catch (error) {
        console.error('Error al obtener el costo:', error);
        alert('Ocurrió un error al intentar obtener el costo.');
    }
});

// Función para pagar una cotización
async function pagarCotizacion(numeroPoliza) {
    if (confirm("¿Estás seguro de que deseas pagar esta cotización?")) {
        try {
            
            const poliza = {
                PolizaId: numeroPoliza, 
                Estado: "Activo"  
            };

            // Realizar la solicitud POST al controlador para descartar la cotización
            const response = await fetch('/Gestiones/Polizas_PolizaPagar', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(poliza),
            });
           
            if (response.ok) {
                const data = await response.json();

                if (data.success) {                    
                    window.location.reload();  
                } else {
                    alert(data.message); 
                }
            } else {
                alert('Error al intentar pagar la cotización, intente nuevamente.');
            }
        } catch (error) {
            console.error('Error al pagar la cotización:', error);
            alert('Ocurrió un error al intentar descartar la cotización.');
        }
    }
}

// Función para descartar una cotización
async function descartarCotizacion(numeroPoliza) {
    if (confirm("¿Estás seguro de que deseas descartar esta cotización?")) {
        try {

            const poliza = {
                PolizaId: numeroPoliza,
                Estado: "No Adquirida"
            };

            // Realizar la solicitud POST al controlador para descartar la cotización
            const response = await fetch('/Gestiones/Polizas_PolizaNoAdquirir', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(poliza),
            });

            if (response.ok) {
                const data = await response.json();

                if (data.success) {
                    window.location.reload();
                } else {
                    alert(data.message);
                }
            } else {
                alert('Error al intentar descartar la cotización, intente nuevamente.');
            }
        } catch (error) {
            console.error('Error al descartar la cotización:', error);
            alert('Ocurrió un error al intentar descartar la cotización.');
        }
    }
}

// Función para realizar una nueva cotización
async function realizarCotizacionv2() {
    const tipoPoliza = parseInt(document.getElementById("tipoPoliza").value);
    const fechaAdquisicion = document.getElementById("fechaAdquisicion").value;
    const costo = parseFloat(document.getElementById("costo").value);

    // Validación de los campos
    if (isNaN(tipoPoliza) || isNaN(costo) || !fechaAdquisicion) {
        alert("Por favor, complete todos los campos correctamente.");
        return;
    }

    // Crear el objeto de la póliza
    const poliza = {
        PolizaId: 0, // Asignamos 0 por defecto ya que es un nuevo registro
        UsuarioId: 0, // El servidor asignará el UsuarioId según la sesión
        TipoPolizaId: tipoPoliza,
        FechaAdquisicion: fechaAdquisicion,
        CostoPago: costo,
        Estado: "Pendiente Pago" // El estado se asigna por defecto
    };

    try {
        // Realizar la solicitud POST al controlador
        const response = await fetch('/Gestiones/Polizas_CrearPoliza', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(poliza),
        });

        // Revisar la respuesta del servidor
        if (response.ok) {
            const data = await response.json();

            if (data.success) {
                //alert('Cotización generada con éxito');
                window.location.reload();
            } else {
                alert(data.message); // Mostrar el mensaje de error recibido
            }
        } else {
            alert('Error al generar la cotización, intente nuevamente.');
        }
    } catch (error) {
        console.error('Error al realizar la cotización:', error);
        alert('Ocurrió un error al intentar realizar la cotización.');
    }
}