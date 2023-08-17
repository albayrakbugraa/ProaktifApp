
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

document.getElementById("pageSizeSelect").addEventListener("change", function () {
    document.getElementById("pageSizeForm").submit();
});




