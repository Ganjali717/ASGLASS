$(document).ready(function () {
    $(document).on("click", ".btn-item-delete", function (e) {
        e.preventDefault();

        var url = $(this).attr("href");

        Swal.fire({
            title: 'Əminsiniz?',
            text: "Sildikdən sonra fayli geri qaytarmaq olmayacaq!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Bəli, Sil!'
        }).then((result) => {

            if (result.isConfirmed)
            {
                fetch(url)
                    .then(response => response.json())
                    .then(data => {
                        console.log(data)
                        if (data.status == 200) {
                            window.location.reload(true)
                        }
                        else {
                            console.log(data.status)
                            Swal.fire(
                                'Error!',
                                'Sizin fayl silinmədi!',
                                'error'
                            )
                        }
                    })

            }
        })


    })



    $(document).on("click", ".remove-img-box", function (e) {
        e.preventDefault();
        $(this).parent().remove()
    })


    $(document).on("click", ".single-btn", function (e) {
        e.preventDefault()
    })

  
    var connection = new signalR.HubConnectionBuilder().withUrl("/asglasshub").build();

    $(connection).on("ChangeOnlineStatus", function (id, status) {
        var statusStr = status == true ? "online" : "offline";
        $("#" + id).text(statusStr)

    })

})
