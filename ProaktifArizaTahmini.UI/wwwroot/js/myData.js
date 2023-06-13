document.getElementById('uploadExcel').addEventListener('submit', function (e) {
    e.preventDefault();
    var form = e.target;
    var formData = new FormData(form);

    var xhr = new XMLHttpRequest();
    xhr.open("POST", "/MyData/List", true);
    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4 && xhr.status === 200) {
            // Yükleme tamamlandıktan sonra yapılacak işlemler
            console.log(xhr.responseText);
        }
    };
    xhr.send(formData);
    setTimeout(function () {
        location.reload();
    }, 3500)
});

//function changePageSize() {
//    var pageSizeSelect = document.getElementById("pageSizeSelect");
//    var pageSize = pageSizeSelect.value;

//    var form = document.getElementById("pageSizeForm");
//    form.action = "/mydata/list?page=1&pageSize=" + pageSize;
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

document.getElementById("pageSizeSelect").addEventListener("change", function () {
    document.getElementById("pageSizeForm").submit();
});