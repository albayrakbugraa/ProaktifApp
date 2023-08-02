document.getElementById('uploadExcel').addEventListener('submit', function (e) {
    e.preventDefault();
    var form = e.target;
    var formData = new FormData(form);

    var xhr = new XMLHttpRequest();
    xhr.open("POST", "/MyData/ImportExcel", true);
    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4 && xhr.status === 200) {
            // Yükleme tamamlandıktan sonra yapılacak işlemler
            //console.log(xhr.responseText);
            //console.log(formData);
        }
    };
    xhr.send(formData);
    setTimeout(function () {
        location.reload();
    }, 3500)
});