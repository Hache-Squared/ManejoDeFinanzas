function inicializarFormularioTransacciones(urlObtenerCategorias) {
    $("#TipoOperacionId").change(async function () {
        const valorSeleccionado = $(this).val();

        const respuesta = await fetch(urlObtenerCategorias, {
            method: "POST",
            body: valorSeleccionado,
            headers: {
                'Content-type': 'application/json'
            }
        })

        const json = await respuesta.json();
        console.log(json)

        if (!json) {
            return;
        }
        const opciones = json.map(categoria => `<option value=${categoria.value}> ${categoria.text} </option>`);

        $("#CategoriaId").html(opciones);




    })
}