let nav = document.querySelector("nav");
window.addEventListener("scroll", function () {
  if (window.scrollY > 60) {
    nav.classList.add("fixed");
  } else {
    nav.classList.remove("fixed");
  }
});

var cycle = 1;
var oldactive = document.querySelector(".carousel-item.active");
var thisimg = oldactive.firstElementChild;
$(thisimg).css("transform", "scale(1.2)");
$(thisimg).css("transition", "all 6s ease");

$("#carouselExampleControls").on("slide.bs.carousel", function () {
  if (cycle == 1) {
    $(thisimg).css("transform", "scale(1)");
    oldactive.nextElementSibling.firstElementChild.style.transform = "scale(1)";
    setTimeout(() => {
      oldactive.nextElementSibling.firstElementChild.style.transform =
        "scale(1.2)";
      oldactive.nextElementSibling.firstElementChild.style.transition =
        "all 6s ease";
    }, 1000);

    cycle = 0;
  } else {
    $(thisimg).css("transform", "scale(1)");
    oldactive.firstElementChild.style.transform = "scale(1)";

    setTimeout(() => {
      oldactive.firstElementChild.style.transform = "scale(1.2)";
      oldactive.firstElementChild.style.transition = "all 6s ease";
    }, 1000);

    cycle++;
  }
});

$(".owl-carousel.owl-theme").owlCarousel({
  loop: false,
  margin: 30,
  responsive: {
    0: {
      items: 1,
    },
    600: {
      items: 2,
    },
    1000: {
      items: 3,
    },
  },
});

$(".owl-carousel.disc").owlCarousel({
  loop: true,
  margin: 0,
  dotsEach: 2,
  responsive: {
    0: {
      items: 1,
    },
    600: {
      items: 1,
    },
    1000: {
      items: 1,
    },
  },
});
