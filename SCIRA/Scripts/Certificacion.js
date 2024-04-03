function FileName(id) {
    var filename = $("#f" + id).val();
    $("#nb_a" + id).val(filename);
}

function ResetFileName(id) {
    $("#f" + id).val("");
    $("#nb_a" + id).val("");
}