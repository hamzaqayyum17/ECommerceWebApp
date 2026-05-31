// AJAX Live Search
const searchInput = document.getElementById('liveSearch');
const resultsBox = document.getElementById('searchResults');

if (searchInput) {
    searchInput.addEventListener('input', function () {
        const keyword = this.value.trim();

        if (keyword.length < 2) {
            resultsBox.innerHTML = '';
            resultsBox.style.display = 'none';
            return;
        }

        fetch('/Home/SearchSuggestions?keyword=' +
            encodeURIComponent(keyword))
            .then(res => res.json())
            .then(data => {
                if (data.length === 0) {
                    resultsBox.style.display = 'none';
                    return;
                }

                resultsBox.innerHTML = data.map(p => `
                    <a href="/Home/Detail/${p.productId}" 
                       class="list-group-item list-group-item-action">
                        <img src="${p.imageUrl}" width="35" height="35"
                             style="object-fit:cover" class="me-2 rounded"/>
                        <span>${p.name}</span>
                        <span class="float-end text-success fw-bold">
                            Rs. ${p.price}
                        </span>
                    </a>`).join('');

                resultsBox.style.display = 'block';
            });
    });

    document.addEventListener('click', function (e) {
        if (!searchInput.contains(e.target)) {
            resultsBox.style.display = 'none';
        }
    });
}