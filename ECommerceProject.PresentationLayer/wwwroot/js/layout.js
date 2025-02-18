document.addEventListener("DOMContentLoaded", function () {
const menuButton = document.getElementById("menu-button");
const mobileMenu = document.getElementById("mobile-menu");
const closeMenu = document.getElementById("close-menu");

// Menü açma butonu
menuButton.addEventListener("click", function () {
    mobileMenu.classList.remove("hidden");
});

// Menü kapatma butonu
closeMenu.addEventListener("click", function () {
    mobileMenu.classList.add("hidden");
});

// Menü dışına tıklayınca kapatma
mobileMenu.addEventListener("click", function (event) {
if (event.target === mobileMenu) {
    mobileMenu.classList.add("hidden");
}
});
});
