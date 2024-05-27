"use strict";

const sortBtn = document.querySelector(".sortcontainer");
const sortsContainer = document.querySelector(".allsortlist");
var filterContainers = document.querySelectorAll('.eachfilterinnercontainer');

filterContainers.forEach(function(container) {
    container.addEventListener('click', function() {
        // Toggle the class 'active' on the next sibling element
        this.nextElementSibling.classList.toggle('show');
    });
});

sortBtn.addEventListener('click', () => {
    sortsContainer.classList.toggle('hidden');
});
