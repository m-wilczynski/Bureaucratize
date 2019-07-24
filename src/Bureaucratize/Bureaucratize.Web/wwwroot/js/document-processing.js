(function () {
    var image = $("#filled_document").find("img")[0];
    image.onload = function () {

        var imageWidthScale = image.clientWidth / image.naturalWidth;
        var imageHeightScale = image.clientHeight / image.naturalHeight;


        var areas = $('[id^=area-]');
        for (var i = 0; i < areas.length; i++) {

            var initialWidth = $(areas[i]).width();
            $(areas[i]).width(initialWidth * imageWidthScale);

            var initialHeight = $(areas[i]).height();
            $(areas[i]).height(initialHeight * imageHeightScale);

            var initialTop = areas[i].offsetTop;
            areas[i].style["top"] = (initialTop * imageHeightScale) + "px";

            var initialLeft = areas[i].offsetLeft;
            areas[i].style["left"] = ((initialLeft * imageWidthScale) + image.offsetLeft) + "px";

            areas[i].style["z-index"] = "2";
        }

    }
}());


function setupProcessing (documentId) {

    $("#start-processing")[0].onclick = function () {

        $.ajax({
            url: "StartDocumentProcessing/" + documentId,
            success: function () {

                "use strict";

                $("#start-processing")[0].disabled = true;
                $("#start-processing")[0].innerText = "Przetwarzanie...";

                var innerRelativePath = window.location.pathname.split('/')[1] !== "Home"
                    ? "/" + window.location.pathname.split('/')[1] + "/"
                    : "";

                var wsUri = "ws://" + window.location.host + innerRelativePath + "?documentId=" + documentId;
                var socket = new WebSocket(wsUri);

                socket.onmessage = function (e) {

                    var data = JSON.parse(e.data);

                    switch (data.Scope) {

                        case "PageArea":

                            var areaIdentifier = data.AreaName.replace(/\s/g, "");

                            var messageHtml = '<div class="document-area-update">';
                            messageHtml += '<div class="document-area-update-title">';
                            messageHtml += '<h5 class="card-title">' + data.AreaName + '</h5>';
                            messageHtml += '<span class="badge badge-primary">' + data.AreaTypeText + '</span>';
                            messageHtml += '</div>';
                            messageHtml += '<div><div class="input-group mb-3 document-area-update-result">';
                            messageHtml +=
                                '<input id="message-' + areaIdentifier + '" type="text" class="form-control" aria-describedby="basic-addon2" value="' +
                                data.ResultStringified +
                                '" readonly>';
                            messageHtml += '</div></div>';

                            $("#document_messages").append(messageHtml);

                            $("#message-" + areaIdentifier)[0].onclick = function () {
                                $('#area-' + areaIdentifier).tooltip('toggle');
                            };

                            $("#message-" + areaIdentifier)[0].onblur = function () {
                                $('#area-' + areaIdentifier).tooltip('hide');
                            };

                            var areaTooltip = $("#area-" + areaIdentifier)[0];
                            var popupText = areaTooltip.attributes["data-original-title"].nodeValue;
                            areaTooltip.attributes["data-original-title"].nodeValue =
                                popupText.replace(/{Nieprzetworzony}/g, data.ResultStringified);

                            break;

                        case "Page":
                            $("#processing-modal #inquiryType").text(data.Result.Type || "PUSTY");
                            $("#processing-modal #name").text(data.Result.Customer.Name || "PUSTY");
                            $("#processing-modal #surname").text(data.Result.Customer.Surname || "PUSTY");
                            $("#processing-modal #pesel").text(data.Result.Customer.PESEL || "PUSTY");

                            for (var i = 0; i < data.Errors.length; i++) {
                                $("#processing-modal #modal-errors").append("<li>" + data.Errors[i] + "</li>");
                            }

                            if (data.Errors.length === 0) {
                                $("#processing-modal #modal-errors").hide();
                            }

                            $("#processing-modal").modal('show');
                            break;
                        case "Document":
                            $("#start-processing")[0].innerText = "Dokument przetworzony";
                            break;
                    }
                };
            }
        });
    };
}
