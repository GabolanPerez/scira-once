$(document).ready(function () {
    $("#fileUploader").dropzone({
        maxFilesize: 700,
        thumbnailWidth: 100,
        thumbnailHeight: 100,
        init: function () {

            this.on("addedfile", function (file) {

            });
            this.on("success", function (file, response) {
                // Create the remove button
                var removeButton = Dropzone.createElement("<button class=' btn btn-default btn-sm' title='Eliminar'><i class='fa fa-times'></i></button>");

                // Capture the Dropzone instance as closure.
                var _this = this;

                //Obtenemos el id del elemento
                var element_id = _this.element.id;


                // Listen to the click event
                removeButton.addEventListener("click", function (e) {
                    // Make sure the button click doesn't submit the form:
                    e.preventDefault();
                    e.stopPropagation();

                    // Remove the file preview.
                    _this.removeFile(file);
                    // If you want to the delete the file on the server as well,
                    // you can do the AJAX request here.
                    $.ajax({
                        dataType: "html",
                        type: "post",
                        url: '/Archivos/Delete',
                        data: { uuid: file.upload.uuid },
                        timeout: 600000, // estbalecer timeout a 600 segundos (10 miutos)
                        success: function (data) {
                            if (data === 0) return;
                            //eliminar el id del archivo del array
                            var elemento = $("#" + element_id);
                            var target = elemento.attr("data-target");

                            if (target !== null) {
                                var values = $('#' + target).val();

                                var filtered = values.filter(function (value, index, arr) {
                                    return value !== data;
                                });

                                $('#' + target).val(filtered);
                            }

                        },
                        error: function (xhr, ajaxOptions, thrownError) {

                        }
                    });
                });

                // Add the button to the file preview element.
                file.previewElement.appendChild(removeButton);


                getResponse(file, response, this.element.id);
            });

            this.on("sending", function (file, xhr, formData) {
                formData.append('uuid', file.upload.uuid);
            });
        }
    });
});



function getResponse(file, response, element_id) {
    if (response === 0) return;
    //Añadir el id del archivo a el array de ids de archivos
    var elemento = $("#" + element_id);
    var target = elemento.attr("data-target");

    if (target !== null) {
        var option = new Option(file.name, response); $('#' + target).append($(option));
        var values = $('#' + target).val();
        if (values === null) {
            values = [];
        }

        values.push(response);

        $('#' + target).val(values);
    }
}
