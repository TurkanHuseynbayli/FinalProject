let productInput;
$(document).on('keyup', `#product-search-input`, function () {
    console.log("alindi")
    productInput = $(this).val().trim();
    $("#new-products").empty();
    $("#old-products").css("display", "flex")
    if (productInput.length > 0) {
        $("#old-products").css("display", "none")
        $.ajax({
            url: '/Product/ProductSearch/',
            data: { "search": productInput },
            type: 'Get',
            success: function (res) {
                $("#new-products").append(res)
            }
        })
    }

})






   


