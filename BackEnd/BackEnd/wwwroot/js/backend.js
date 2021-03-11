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
//$("#remove").click(function () {
//  $.ajax({
//    url: '/Basket/RemoveCount/',
//    type: 'Post',
//    data: {
//          id: id
//      },
//     success: function () {
//          prompt("Remove to cart");
//      }
  
//})

//});

//$("#removeItem").click(function () {
//    $.ajax({
//        url: '/Basket/RemoveItem/',
//        type: 'Post',
//        data: {
//            id: id
//        },
//        success: function () {
//            prompt("Remove to cart");
//        }

//    })

//});



    //$("#btnComment").click(function () {
    //    $.ajax({
    //        url: '/Account/LeaveComment/',
    //        type: "Post",
    //        data: $("#commentForm").serialize()
    //    }).done(function (response) {
    //        debugger;
    //        if (response.Success) {
    //            window.location.reload();
    //        }
    //        else {
    //            swal("Error!", response.Message, "error");
    //        }

    //    }).fail(function)
    //});


