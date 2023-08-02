

function searchTable1() {
    // Arama metnini alın
    var input = document.getElementById("searchInput");
    var filter = input.value.toUpperCase();

    // Tablo satırlarını alın
    var table = document.getElementById("example");
    var rows = table.getElementsByTagName("tr");

    // Her bir satırı döngüye alın
    for (var i = 0; i < rows.length; i++) {
        var row = rows[i];

        // Arama metnini içeren hücreleri kontrol edin
        var cells = row.getElementsByTagName("td");
        var found = false;
        for (var j = 0; j < cells.length; j++) {
            var cell = cells[j];
            if (cell) {
                var cellText = cell.textContent || cell.innerText;
                if (cellText.toUpperCase().indexOf(filter) > -1) {
                    found = true;
                    break;
                }
            }
        }

        // Satırı göster veya gizle
        if (found) {
            row.style.display = "";
        } else {
            row.style.display = "none";
        }
    }
}


// downloadButton class'ına sahip tüm butonları seçin
var downloadButtons = document.getElementsByClassName('downloadButton');
console.log(downloadButtons)
// Her bir buton için olay dinleyicisi ekleyin
for (var i = 0; i < downloadButtons.length; i++) {
    downloadButtons[i].addEventListener('click', function (e) {
        e.preventDefault();

        // İlgili satırdaki dosya yollarını alın
        var rowIndex = this.closest('tr').rowIndex;
        console.log(rowIndex)
        var filePaths = document.querySelectorAll('#example tbody tr:nth-child(' + rowIndex + ') #dynamicFilePaths input[type="hidden"]');
        console.log(filePaths)

        // Form verilerini oluşturun
        var formData = new FormData();
        filePaths.forEach(function (input) {
            formData.append(input.id, input.value);
        });

        // AJAX isteği gönderin
        var xhr = new XMLHttpRequest();
        xhr.open('POST', '/Disturbance/DownloadFiles');
        xhr.responseType = 'blob';
        xhr.onload = function () {
            if (xhr.status === 200) {
                // İndirilen dosyayı kullanıcıya sunabilirsiniz
                var a = document.createElement('a');
                var url = window.URL.createObjectURL(xhr.response);
                a.href = url;
                a.download = 'comtrade.zip';
                document.body.appendChild(a);
                a.click();
                document.body.removeChild(a);
                window.URL.revokeObjectURL(url);
            } else {
                // İsteğin hata aldığı durumda yapılacak işlemler
                console.log("HATA")
            }
        };
        xhr.send(formData);
    });
}

//function changePageSize() {
//    var pageSizeSelect = document.getElementById("pageSizeSelect");
//    var pageSize = pageSizeSelect.value;

//    var form = document.getElementById("pageSizeForm");
//    form.action = "/disturbances/filter?page=1&pageSize=" + pageSize;
//    localStorage.setItem("pageSize", pageSize);
//    form.submit();
//}

//window.onload = function () {
//    var pageSizeSelect = document.getElementById("pageSizeSelect");
//    var savedPageSize = localStorage.getItem("pageSize");

//    if (savedPageSize) {
//        pageSizeSelect.value = savedPageSize;
//    }
//};


//function applyDateFilter() {
//    var startDate = document.getElementById("startDatetimepicker").value;
//    var endDate = document.getElementById("endDatetimepicker").value;
//    var hiddenFaultStartDate = document.getElementById("hiddenFaultStartDate").value
//    var hiddenFaultEndDate = document.getElementById("hiddenFaultStartDate").value
//    hiddenFaultStartDate = startDate;
//    hiddenFaultEndDate = endDate;
//    localStorage.setItem("hiddenFaultStartDate", startDate);
//    localStorage.setItem("hiddenFaultEndDate", endDate);

//    document.getElementById("hiddenFaultEndDate").value = endDate;
//    var pageSizeSelect = document.getElementById("pageSizeSelect");
//    var pageSize = pageSizeSelect.value;

//    var form = document.getElementById("pageSizeForm");
//    form.action = "/disturbances/filter?page=1&pageSize=" + pageSize;
//    localStorage.setItem("pageSize", pageSize);
//    form.submit();
//}

function saveToLocalStorage() {
    var startDate = document.getElementById("startDatetimepicker").value;
    var endDate = document.getElementById("endDatetimepicker").value;

    localStorage.setItem("hiddenFaultStartDate", startDate);
    localStorage.setItem("hiddenFaultEndDate", endDate);
}


function toISOString(dateString) {
    var dateParts = dateString.split(".");
    var year = parseInt(dateParts[2]);
    var month = parseInt(dateParts[1]);
    var day = parseInt(dateParts[0]);

    // ISO 8601 formatına dönüştürme
    var isoDate = year.toString().padStart(4, '0') + '-' + month.toString().padStart(2, '0') + '-' + day.toString().padStart(2, '0') + 'T00:00:00.000Z';

    return isoDate;
}


function applyDateFilter() {
    var startDateInput = document.getElementById("startDatetimepicker").value;
    var endDateInput = document.getElementById("endDatetimepicker").value;
    var pageSizeSelect = document.getElementById("pageSizeSelect");
    var pageSize = pageSizeSelect.value;

    // Tarihleri ISO 8601 formatına dönüştür
    var startDateISO = toISOString(startDateInput);
    var endDateISO = toISOString(endDateInput);

    if (new Date(startDateISO).getTime() > new Date("0001-02-01T00:00").getTime() && new Date(endDateISO).getTime() > new Date("0001-02-01T00:00").getTime()) {
        // localStorage'a değerleri kaydet
        localStorage.setItem("hiddenFaultStartDate", startDateISO);
        localStorage.setItem("hiddenFaultEndDate", endDateISO);
        localStorage.setItem("pageSize", pageSize);

        // FormData oluşturun ve parametreleri ekleyin
        var formData = new FormData();
        formData.append("page", "1");
        formData.append("pageSize", pageSize);
        formData.append("FaultStartDate", startDateISO);
        formData.append("FaultEndDate", endDateISO);

        var xhr = new XMLHttpRequest();
        xhr.open("POST", "/disturbances/filterpagesize", true);
        xhr.onreadystatechange = function () {
            if (xhr.readyState === 4 && xhr.status === 200) {
                // Yükleme tamamlandıktan sonra yapılacak işlemler
                console.log(xhr.responseText);
            }
        };

        xhr.send(formData);
    } else {
        var newEndDate = new Date("9999-01-01T00:00");
        var newStartDate = new Date("0001-01-01T00:00");

        // localStorage'a değerleri kaydet
        localStorage.setItem("pageSize", pageSize);

        // FormData oluşturun ve parametreleri ekleyin
        var formData = new FormData();
        formData.append("page", "1");
        formData.append("pageSize", pageSize);
        formData.append("FaultStartDate", newStartDate.toISOString());
        formData.append("FaultEndDate", newEndDate.toISOString());

        var xhr = new XMLHttpRequest();
        xhr.open("POST", "/disturbances/filterpagesize", true);
        xhr.onreadystatechange = function () {
            if (xhr.readyState === 4 && xhr.status === 200) {
                // Yükleme tamamlandıktan sonra yap
                console.log(xhr.responseText);
            }
        };

        xhr.send(formData);
    }
}


document.getElementById("pageSizeSelect").addEventListener("change", function () {
    document.getElementById("pageSizeForm").submit();
});




