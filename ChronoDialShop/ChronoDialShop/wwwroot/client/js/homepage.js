"use strict";

var swiper = new Swiper(".mySwiper", {
    slidesPerView: 1,
    spaceBetween: 30,
    loop: true,
    // autoplay: {
    //     delay: 2500,
    // },
    pagination: {
      el: ".swiper-pagination",
      clickable: true,
    },
});


var swiper = new Swiper(".numberDialSwiper", {
    slidesPerView: 1,
    loop: true,
    pagination: {
      el: ".swiper-pagination",
      clickable: true,
    },
    breakpoints: {
      576: {
        slidesPerView: 2,
      },
      992: {
        slidesPerView: 3,
      }
    },
  });

document.addEventListener("DOMContentLoaded", function() {
    const boxes = document.querySelectorAll(".eachcategory");
    const productContainers = document.querySelectorAll(".eachcategory-productcontainer");

    boxes.forEach(function(box, index) {
        box.addEventListener("click", function() {
             productContainers.forEach(function(container) {
                container.classList.add("hidden");
            });
            
            productContainers[index].classList.remove("hidden");
        });
    });
});

var swiper = new Swiper(".newproductcontainer", {
    slidesPerView: 1,
    loop: true,
    pagination: {
      el: ".swiper-pagination",
      clickable: true,
    },
    breakpoints: {
      576: {
        slidesPerView: 2,
      },
      992: {
        slidesPerView: 3,
        spaceBetween: 20,
      }
    },
  });

  var swiper = new Swiper('.swiper-container', {
    // Optional parameters
    loop: true,
    autoplay: {
      delay: 5000,
    },
    // If you need pagination
    pagination: {
      el: '.swiper-pagination',
      clickable: true,
    },
  });

// const eachCategory = document.querySelectorAll('.eachcategoryactive');

// document.addEventListener('DOMContentLoaded', function() {
//   eachCategory.forEach(ec => {
//       ec.addEventListener('click', function() {
//           ec.forEach(oec => {
//               if (oec !== this) {
//                   otherDiv.classList.remove('eachcategoryactive');
//               }
//           });
//           this.classList.toggle('eachcategoryactive');
//       });
//   });
// });