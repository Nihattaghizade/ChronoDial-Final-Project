"use strict";

const menuIcon = document.querySelectorAll(".menubutton"); 
const menuContainer = document.querySelector(".menucontainer");
const menuCloseBtn = document.querySelectorAll(".menuCloseButton");

const searchBtn = document.querySelectorAll(".searchButton");
const searchSection = document.querySelector(".searchsection");
const searchCloseBtn = document.querySelectorAll(".searchCloseButton");

const menuConPageBoxInner = document.querySelectorAll(".pagebox");
const menuList = document.querySelector(".menulist");
const pageList = document.querySelector(".pagelist");

const basketOpenBtn = document.querySelectorAll(".cart-button");
const basketContainer = document.querySelector(".basketcontainer");
const basketCloseBtn = document.querySelectorAll(".basketCloseButton");

menuIcon.forEach( (e) => {
    e.addEventListener("click", () => {
        menuContainer.classList.remove("hidden");
    });
});

menuCloseBtn.forEach( (e) => {
    e.addEventListener("click", () => {
        menuContainer.classList.add("hidden");
    });
});

basketOpenBtn.forEach( (e) => {
  e.addEventListener("click", () => {
      basketContainer.classList.remove("hidden");
  });
});

basketCloseBtn.forEach( (e) => {
  e.addEventListener("click", () => {
      basketContainer.classList.add("hidden");
  });
});

searchBtn.forEach( (e) => {
    e.addEventListener("click", () => {
        searchSection.classList.remove("hidden");
    });
});

searchCloseBtn.forEach( (e) => {
    e.addEventListener("click", () => {
        searchSection.classList.add("hidden");
    });
});

menuConPageBoxInner.forEach( (e) => {
    e.addEventListener("click", () => {
        menuList.classList.toggle("hidden");
        pageList.classList.toggle("hidden");
    });
});



// const decreaseBtn = document.querySelector('.decreasebtn');
// const increaseBtn = document.querySelector('.increasebtn');
// const counterInput = document.getElementById('counter');

// document.addEventListener('DOMContentLoaded', (event) => {
//     const decreaseBtn = document.querySelectorAll('.decreasebtn');
//     const increaseBtn = document.querySelectorAll('.increasebtn');
//     const counterInput = document.querySelectorAll('.counter');

//     increaseBtn.addEventListener('click', () => {
//         counterInput.value = parseInt(counterInput.value) + 1;
//     });

//     decreaseBtn.addEventListener('click', () => {
//         let currentValue = parseInt(counterInput.value);
//         if (currentValue > 0) {
//             counterInput.value = currentValue - 1;
//         }
//     });
// });


// increaseBtn.addEventListener("click", () => {
    
//         counterInput.value++;
    
// });

// decreaseBtn.addEventListener("click", () => { 
    

//     if(counterInput >= 2){
//         counterInput.value--;
//     }
// });

document.addEventListener('DOMContentLoaded', (event) => {
    const decreaseBtn = document.querySelector('.decreasebtn');
    const increaseBtn = document.querySelector('.increasebtn');
    const counterInput = document.getElementById('counter');

    increaseBtn.addEventListener('click', () => {
        counterInput.value = parseInt(counterInput.value) + 1;
    });

    decreaseBtn.addEventListener('click', () => {
        let currentValue = parseInt(counterInput.value);
        if (currentValue > 1) {
            counterInput.value = currentValue - 1;
        }
    });
});