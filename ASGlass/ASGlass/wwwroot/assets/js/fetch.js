$(document).ready(function () {
    $(".product-search").keyup(function (e) {
        e.preventDefault()

        var search = $(this).val();

        fetch('https://localhost:44393/shop/SearchFilter?search=' + search)
            .then(response => response.text())
            .then(data => {
                $('#search-list-box').html(data);
            })
    });
});