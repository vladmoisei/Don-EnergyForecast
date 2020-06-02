$(function () {


    // Verificare daca este small screen
    let isSmallScren = (screen.width <= 700) ? "True" : "False";

    var link = document.getElementById("downloadToExcel").getAttribute('href');
    var dataFrom;
    var dataTo;
    var hrefLink;
    var ancorElem = document.getElementById("downloadToExcel");
    // Prevent default event la click link raportare daca nu e introdusa data
    ancorElem.addEventListener("click", function (e) {
        if (dataFrom === undefined || dataTo === undefined) {
            alert("Te rog selecteaza perioada de raportare.");
            e.preventDefault();
        }

    });

    //Date time picker options
    var dateFormat = "dd.mm.yy",
        from = $("#from")
            .datepicker({
                defaultDate: "+0w",
                changeMonth: true,
                numberOfMonths: 2
            })
            .on("change", function () {
                to.datepicker("option", "minDate", getDate(this));
                //
                console.log("S-a schimbat data la from");
                dataFrom = document.getElementById("from").value;
                hrefLink = link + "?dataFrom=" + dataFrom + "&dataTo=" + dataTo; // Pass parameters to link
                console.log(hrefLink);
                document.getElementById("downloadToExcel").setAttribute('href', hrefLink);
            }),
        to = $("#to").datepicker({
            defaultDate: "+0w",
            changeMonth: true,
            numberOfMonths: 2
        })
            .on("change", function () {
                from.datepicker("option", "maxDate", getDate(this));
                //
                console.log("S-a schimbat data la to");
                dataTo = document.getElementById("to").value;
                hrefLink = link + "?dataFrom=" + dataFrom + "&dataTo=" + dataTo;
                console.log(hrefLink);
                document.getElementById("downloadToExcel").setAttribute('href', hrefLink);

                // Adaugare de mine pentru selectare date de afisat automat in functie de interval selectat
                // Creare element lista selectie afisare date
                let selectieAfisareDateElement = document.getElementById("selectieAfisareDate");

                //alert("S-a schimbat data To. EventListener");
                // Ajax Call to function on server
                $.ajax({
                    url: " /Test/_IndexPlc",
                    type: 'POST',
                    data: {
                        dataFrom: dataFrom,
                        dataTo: dataTo
                    },
                    success: function (response) {
                        console.log("S-a realizat event selectie dataPicker");
                        $(".table").html(response);
                    },
                    error: function (response) {
                        console.log("Eroare JS la event selectie dataPicker");
                    }

                });
            });

    function getDate(element) {
        var date;
        try {
            date = $.datepicker.parseDate(dateFormat, element.value);
        } catch (error) {
            date = null;
        }

        return date;
    }

    // Set format of data
    $("#from").datepicker("option", "dateFormat", "dd.mm.yy");
    $("#to").datepicker("option", "dateFormat", "dd.mm.yy");

});