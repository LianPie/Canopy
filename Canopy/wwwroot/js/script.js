document.addEventListener("DOMContentLoaded", function () {
    const cityBtn = document.getElementById("cityBtn");
    const selectedCity = document.getElementById("selectedCity");
    const icon = document.getElementById("icon-email");
    const dropdownContent = document.querySelector(".dropdown-content");
    const cityLinks = dropdownContent.querySelectorAll("a");

    cityBtn.addEventListener("click", function (event) {
        event.stopPropagation();
        dropdownContent.classList.toggle("show");
    });

    cityBtn.addEventListener("mouseenter", function () {
        dropdownContent.classList.remove("show");
    });

    cityLinks.forEach((link) => {
        link.addEventListener("click", function (event) {
            event.preventDefault();
            const cityName = this.getAttribute("data-city");
            selectedCity.textContent = cityName;
            dropdownContent.classList.remove("show");
            icon.classList.toggle("fa-chevron-down");
            icon.classList.toggle("fa-chevron-up");
        });
    });

    dropdownContent.addEventListener("click", function (event) {
        event.stopPropagation();
    });

    document.addEventListener("click", function () {
        dropdownContent.classList.remove("show");
    });
});

$(document).ready(function () {
    $('.dropdown-submenu a.test').on("click", function (e) {
        $(this).next('ul').toggle();
        e.stopPropagation();
        e.preventDefault();
    });
});
$(document).ready(function () {
    // Prevent closing the dropdown when clicking inside it
    $(".section-dropdown").on("click", function (event) {
        event.stopPropagation();
    });

    // Toggle the dropdown on checkbox change
    $("#dropdown").on("change", function () {
        if ($(this).prop("checked")) {
            $(".section-dropdown").show();
        } else {
            $(".section-dropdown").hide();
        }
    });
});