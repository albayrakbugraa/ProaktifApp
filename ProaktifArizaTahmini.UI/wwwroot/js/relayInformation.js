document.getElementById('uploadExcel').addEventListener('submit', function (e) {
    e.preventDefault();
    var form = e.target;
    var formData = new FormData(form);

    var xhr = new XMLHttpRequest();
    xhr.open("POST", "/RelayInformation/ImportExcel", true);
    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4 && xhr.status === 200) {
            // Yükleme tamamlandıktan sonra yapılacak işlemler
            
            // "yükleniyor" mesajını gizle
            document.getElementById('loading-indicator').classList.add('d-none');
            // "Excel'den verileri almak için dosya seçip yükleyin." mesajını göster
            document.getElementById('excel-message').classList.remove('d-none');

            location.reload();            
        }
    };

    // Yükleme başladığında "yükleniyor" mesajını göster
    document.getElementById('loading-indicator').classList.remove('d-none');
    // "Excel'den verileri almak için dosya seçip yükleyin." mesajını gizle
    document.getElementById('excel-message').classList.add('d-none');

    xhr.send(formData);
});


document.getElementById("pageSizeSelect").addEventListener("change", function () {
    document.getElementById("pageSizeForm").submit();
});

