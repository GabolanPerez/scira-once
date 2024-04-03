function deshabilitar() {
    var originalValue = $("#submit").val();
    var originalValueC = $(".submit").val();

    $("#submit").val("...");
    $("#submit").attr("disabled",true);
    $(".submit").val("...");
    $(".submit").attr("disabled", true);

    console.log("Deshabilitando inputs");
    setInterval(function () { habilitar(originalValue, originalValueC); }, 3000, null);

    return true;
}

function habilitar(original,originalC) {
    $("#submit").val(original);
    $("#submit").attr("disabled", false);

    $(".submit").val(originalC);
    $(".submit").attr("disabled", false);

    console.log("Habilitar #" + original + " ." + originalC);

    $("body").removeClass('loading');

    return true;
}

$(document).ready(function () {
    loadPopover();
});

function loadPopover() {
    $('[data-toggle="popover"]').popover({
        container: 'body'
    });
}


function cppc(cadena) {
    var len = cadena.length;
    var i = 0;
    var res = "";
    for (i = 0; i < len; i++) {
        if (cadena.charAt(i) === ',') {
            res += '.';
        }
        else {
            res += cadena.charAt(i);
        }
    }
    return res;
}

function initTables(route, paginate) {

    if (paginate !== false) {
        paginate = true;
    }

    $(route + ' .results').DataTable({
        paginate:paginate,
        responsive: true,
        pagingType: "full_numbers",
        lengthChange: true,
        lengthMenu: [[10, 25, 50, -1], [10, 25, 50, dataTableAll]],
        columnDefs: [
            { orderable: false, targets: -1 }
        ],
        language: dataTableOptions
    });
}


$(document).ready(function () {
    $('.S1').on('keypress', function (e) {
        if (e.keyCode === 101 || e.keyCode === 45 || e.keyCode === 46 || e.keyCode === 43 || e.keyCode === 44 || e.keyCode === 47) {
            return false;
        }
        soloNumeros(e.keyCode);
    });

    $('.S2').on('keypress', function (e) {
        if (e.keyCode === 46 || soloNumeros(e.keyCode)) {
            return false;
        }
    });
});

function soloNumeros(e) {
    var key = window.Event ? e.which : e.keyCode;
    return (key >= 48 && key <= 57);
}

//dar click a elemento
function trigg(elemento) {
    $("#" + elemento).trigger('click');
}


//Obtener resultado desde controlador
function getFromURL(url, idForm) {

    dat = "";
    if (idForm !== null) {
        dat = $("#" + idForm).serialize();
    }

    var res = "";

    $.ajax({
        dataType: "html",
        async: false,
        type: "post",
        cache: false,
        url: url,
        data: dat,
        success: function (data) {
            res = data;
        },
        error: function (xhr, ajaxOptions, thrownError) {
            res = null;
        }
    });

    return res;
}


function executePost(url, idForm,onSucces,onError) {

    if (onSucces === null) {
        onSucces = "";
    }
    if (onError === null) {
        onError = "";
    }


    dat = "";
    if (idForm !== null) {
        dat = $("#" + idForm).serialize();
    }

    var res = "";

    $.ajax({
        dataType: "html",
        async: false,
        type: "post",
        cache: false,
        url: url,
        data: dat,
        success: function (data) {
            eval(onSucces);
            res = data;
        },
        error: function (xhr, ajaxOptions, thrownError) {
            eval(onError);
            res = null;
        }
    });

    return res;
}


var menuModalHistory = [];



//Funciones del modal
 (function (define) {
    define(['jquery'], function ($) {
        return (function () {

            var menuModal = {
                show: show,             //Muestra el modal
                hide: hide,             //Esconde el modal
                sizeSM: sizeSM,         //hace pequeño el modal
                sizeMD: sizeMD,         //hace mediano el modal
                sizeLG: sizeLG,         //hace grande el modal
                setTitle: setTitle,     //Definir el titulo del modal
                bodyLoad: bodyLoad,     //Cargar vista parcial en modal
                error: error,           //Mostrar error
                initModal: initModal    //Inicializa el modal
            };

            return menuModal;

            //#region Accessible Methods

            function initModal(options) {
                $("#menuModal .bodyContent").html("");

                if (options.hasOwnProperty('title')) {
                    setTitle(options.title);
                }
                if (options.hasOwnProperty('size')) {
                    size(options.size);
                }
                if (options.hasOwnProperty('error')) {
                    error(options.error);
                }
                if (options.hasOwnProperty('show')) {
                    if (show) {
                        show();
                    }
                }
                if (options.hasOwnProperty('loadUrl')) {
                    if (options.hasOwnProperty('formID')) {
                        bodyLoad(options.loadUrl, options.formID,null);
                    } else {
                        bodyLoad(options.loadUrl, null,null);
                    }
                }
            }

            function show() {
                $("#menuModal").modal('show');
            }

            function hide() {
                $("#menuModal").modal('hide');
            }

            function sizeSM() {
                size('sm');
            }
            function sizeMD() {
                size('md');
            }
            function sizeLG() {
                size('lg');
            }
            function sizeXL() {
                size('xl');
            }

            function setTitle(title) {
                $("#menuModal .HeadLabel").html(title);
            }

            function error(error) {
                $("#menuModal .modalError").html(error);
            }

            function bodyLoad(url, formID, dat) {
                $("#menuModal .partialModal").html('<div class="row"><div class="col-md-offset-5 col-xs-offset-1 col-md-1 col-xs-1"><span id="ind1" class="IndicatorLoader centered"></span></div></div>');

                if (formID !== null && formID !== "" || dat !== null) {
                    var data = "";
                    if (dat !== null) {
                        data = dat;
                    } else {
                        data = $("#" + formID).serialize();
                    }

                    $.ajax({
                        dataType: "html",
                        type: "get",
                        cache: false,
                        url: url,
                        data: data,
                        success: function (data) {
                            $("#menuModal .partialModal").html(data);
                        },
                        error: function (xhr, ajaxOptions, thrownError) {
                            toastr.error("Algo salió mal al cargar el modal.");
                        }
                    });
                } else {
                    $("#menuModal .partialModal").load(url);
                }
            }
            //#endregion

            //#region Internal Methods
            function removeSize() {
                $("#menuModal .modal-dialog").removeClass('modal-lg');
                $("#menuModal .modal-dialog").removeClass('modal-sm');
                $("#menuModal .modal-dialog").removeClass('modal-md');
            }

            function size(size) {
                removeSize();

                switch (size) {
                    case 'lg':
                        $("#menuModal .modal-dialog").addClass('modal-lg');
                        break;
                    case 'sm':
                        $("#menuModal .modal-dialog").addClass('modal-sm');
                        break;
                    case 'md':
                        $("#menuModal .modal-dialog").addClass('modal-md');
                        break;
                    case 'xl':
                        $("#menuModal .modal-dialog").addClass('modal-xl');
                        break;
                    default:
                        $("#menuModal .modal-dialog").addClass('modal-md');
                        break;
                }
            }
            
            //#endregion

        })();
    });
}(typeof define === 'function' && define.amd ? define : function (deps, factory) {
    if (typeof module !== 'undefined' && module.exports) { //Node
        module.exports = factory(require('jquery'));
    } else {
        window['menuModal'] = factory(window['jQuery']);
    }
}));


//Funciones del modal de confirmación
(function (define) {
    define(['jquery'], function ($) {
        return (function () {

            var action = "";

            var confirmationModal = {
                initModal: initModal,    //Inicializa el modal
                runAction: runAction    //Ejecuta la acción
            };

            return confirmationModal;

            //#region Accessible Methods
                
            function initModal(options) {
                setTitle("Confirmar");
                action = "";

                if (options.hasOwnProperty('title')) {
                    setTitle(options.title);
                }
                if (options.hasOwnProperty('action')) {
                    action = options.action;
                }
                show();
            }

            function runAction() {
                eval(action);
                setTitle("Confirmar");
                action = "";
                hide();
            }
            //#endregion

            //#region Internal Methods
            function show() {
                $("#confirmationModal").modal('show');
            }

            function hide() {
                $("#confirmationModal").modal('hide');
            }

            function setTitle(title) {
                $("#confirmationModal .HeadLabel").html(title);
            }

            //#endregion

        })();
    });
}(typeof define === 'function' && define.amd ? define : function (deps, factory) {
    if (typeof module !== 'undefined' && module.exports) { //Node
        module.exports = factory(require('jquery'));
    } else {
        window['confirmationModal'] = factory(window['jQuery']);
    }
}));



//Funciones del modal de confirmación
(function (define) {
    define(['jquery'], function ($) {
        return (function () {

            var action = "";

            var animation = {
                flash: flash 
            };

            return animation;

            //#region Accessible Methods
            function flash() {
                var flashingDiv = $("<div id='falshingDiv' class='flashingDiv'/>");

                $("body").append(flashingDiv);

                setTimeout(
                    function () {
                        $("#falshingDiv").remove();
                    }, 700);

                

            }
            //#endregion

            //#region Internal Methods

            //#endregion

        })();
    });
}(typeof define === 'function' && define.amd ? define : function (deps, factory) {
    if (typeof module !== 'undefined' && module.exports) { //Node
        module.exports = factory(require('jquery'));
    } else {
        window['animation'] = factory(window['jQuery']);
    }
}));