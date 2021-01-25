$(".owl-carousel.jrc").owlCarousel({
    loop: true,
    margin: 0,
    dotsEach: 2,
    responsive: {
      0: {
        items: 1,
      },
      600: {
        items: 2,
      },
      1000: {
        items: 2.6,
      },
    },
  });

  
  $('.time').each(function() {
    var $this = $(this),
        countTo = $this.attr('data-count');
    
    $({ countNum: $this.text()}).animate({
      countNum: countTo
    },
  
    {
  
      duration: 8000,
      easing:'linear',
      step: function() {
        $this.text(Math.floor(this.countNum));
      },
      complete: function() {
        $this.text(this.countNum);
        //alert('finished');
      }
  
    });  
    
    
  
  });