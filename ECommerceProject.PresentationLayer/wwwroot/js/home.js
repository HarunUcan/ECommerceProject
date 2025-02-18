
        // Menü butonuna tıklandığında menüyü açıp kapat
    

    const categoriesBtn = document.getElementById("categories-btn");
    const dropdown = document.getElementById("dropdown-categories");

        // Hover başladığında göster
        categoriesBtn.addEventListener("mouseenter", () => {
        dropdown.classList.remove("hidden");
        });

        // Fare dropdown'dan çıkınca tekrar gizle
        dropdown.addEventListener("mouseleave", () => {
        dropdown.classList.add("hidden");
        });

        // Kullanıcı dropdown'un içine girerse açık kalsın, çıkarsa kapansın
        categoriesBtn.addEventListener("mouseleave", () => {
        setTimeout(() => {
            if (!dropdown.matches(":hover")) {
                dropdown.classList.add("hidden");
            }
        }, 200);
        });

var swiper = new Swiper(".mySwiper", {
    loop: true,
    autoplay: {
        delay: 3000,
        disableOnInteraction: false,
    },
    pagination: {
        el: ".swiper-pagination",
        clickable: true,
    },
    navigation: {
        nextEl: ".swiper-button-next",
        prevEl: ".swiper-button-prev",
    },
});


