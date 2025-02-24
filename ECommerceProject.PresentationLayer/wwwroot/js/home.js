
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

var productSwiper = new Swiper(".productSwiper", {
    slidesPerView: 2, // Aynı anda görünen ürün sayısı
    spaceBetween: 15,
    slidesPerGroup: 2, // Her tıklamada 2 ürün kaydır
    loop: true,
    loopFillGroupWithBlank: true, // Boşlukları otomatik doldur
    loopedSlides: 6, // Döngüde düzgün çalışması için artırıldı
    autoplay: {
        delay: 2500,
        disableOnInteraction: false,
        waitForTransition: false, // Geçişi bekleme, hemen başlasın
    },
    pagination: {
        el: ".swiper-pagination",
        clickable: true,
    },
    navigation: {
        nextEl: ".swiper-button-next",
        prevEl: ".swiper-button-prev",
    },
    breakpoints: {
        640: {
            slidesPerView: 3,
            spaceBetween: 15,
            slidesPerGroup: 3, // Tabletlerde 3 ürün kaydır
        },
        1024: {
            slidesPerView: 4,
            spaceBetween: 20,
            slidesPerGroup: 4, // Büyük ekranlarda 4 ürün kaydır
        },
        1280: {
            slidesPerView: 5,
            spaceBetween: 25,
            slidesPerGroup: 5, // Daha büyük ekranlarda 5 ürün kaydır
        },
    },
    on: {
        init: function () {
            this.loopFix(); // Başlangıçta boşluk sorununu düzelt
        },
    },
});

