@{
    ViewData["Title"] = "Les espaces";
    var jsonSpaceList = ViewData["JsonSpaceList"] as string;
    var spaces = string.IsNullOrEmpty(jsonSpaceList)
    ? new List<dynamic>()
    : Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(jsonSpaceList);
    var pagination = ViewData["Pagination"] as dynamic;

    // Pagination
    int pageNumber = pagination?.currentPage ?? 1; // Si aucune pagination, on commence par la page 1
    int totalPages = pagination?.totalPages ?? 1;
    int pageSize = pagination?.itemsPerPage ?? 5;

    var pagedSpaces = spaces?.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(); // Espaces paginés
}

@section Scripts {
    <script>
         document.addEventListener('DOMContentLoaded', function () {
             const alertMessage = document.createElement('div');
             alertMessage.classList.add('alert', 'alert-danger', 'alert-dismissible', 'fade', 'show');
             alertMessage.setAttribute('role', 'alert');
             alertMessage.style.display = 'none';
            const modal = document.getElementById('backDropModal');


            function updateDurationDisplay() {
                const startDateInput = document.getElementById("startDate").value;
                const endDateInput = document.getElementById("endDate").value;
                const durationDisplay = document.getElementById("durationDisplay");

                if (startDateInput && endDateInput) {
                    const startDate = new Date(startDateInput);
                    const endDate = new Date(endDateInput);
                    if (endDate > startDate) {
                        const duration = endDate - startDate;
                        const hours = Math.floor(duration / (1000 * 60 * 60));
                        const days = Math.floor(hours / 24);
                        const remainingHours = hours % 24;

                        // Afficher la durée
                        const durationText = days > 0 
                            ? `${days}j et ${remainingHours}h`
                            : `${hours}h`;
                        durationDisplay.textContent = `Durée : ${durationText}`;

                        // Vérifier si la durée est inférieure à 3 heures
                        if (hours < 3) {
                            alertMessage.textContent = "La durée doit être d'au moins 3 heures.";
                            durationDisplay.style.color = "red";
                        } else {
                            alertMessage.textContent = "";
                            durationDisplay.style.color = "green";
                        }
                    } else {
                        alertMessage.textContent = "La date de fin doit être supérieure à la date de début.";
                        durationDisplay.textContent = "";
                    }
                }
            }

            // Ajouter des écouteurs d'événements pour la mise à jour en temps réel
            document.getElementById("startDate").addEventListener("change", updateDurationDisplay);
            document.getElementById("endDate").addEventListener("change", updateDurationDisplay);

            const reservationForm = document.getElementById('reservationForm');
            const saveReservationButton = document.getElementById('saveReservation');
            const buttonValideReservation = document.getElementById('button-valide-reservation');

            const token = localStorage.getItem("jwt");
            const isAuthenticated = token ? true : false;
            if(!isAuthenticated){
                buttonValideReservation.disabled = true;
                alertMessage.innerHTML = `
                        Il faut se connécter avant de faire une réservation !!!  
                        <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
                    `;
                    alertMessage.style.display = 'block';
            }
            modal.addEventListener('show.bs.modal', function (event) {
                const button = event.relatedTarget; 
                const spaceId = button.getAttribute('data-space-id');
                const inputSpaceId = document.getElementById('inputSpaceId');
                inputSpaceId.value = spaceId || '';
            });

            reservationForm.prepend(alertMessage);

            saveReservationButton.addEventListener('click', function (event) {
                event.preventDefault();
                if(!isAuthenticated){
                    buttonValideReservation.disabled = true;
                    alertMessage.innerHTML = `
                        Il faut se connécter avant de faire une réservation !!!  
                        <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
                    `;
                    alertMessage.style.display = 'block';
                }
                else {
                    buttonValideReservation.disabled = false;
                    const startDateInput = document.getElementById('startDate');
                    const endDateInput = document.getElementById('endDate');
    
                    const startDate = new Date(startDateInput.value);
                    const endDate = new Date(endDateInput.value);
    
                    // Réinitialiser l'alerte
                    alertMessage.style.display = 'none';
    
                    if (!startDateInput.value || !endDateInput.value) {
                        alertMessage.innerHTML = `
                            Ces deux champs sont obligatoires !  
                            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
                        `;
                        alertMessage.style.display = 'block';
                        return;
                    }
    
                    if (endDate <= startDate) {
                        alertMessage.innerHTML = `
                            La date de fin doit être supérieure à la date de début !
                            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
                        `;
                        alertMessage.style.display = 'block';
                        return;
                    }
    
                    // Si tout est valide, soumettre le formulaire
                    reservationForm.submit();
                }

            });

        });
    </script>
}

<style>
    .filter-container {
        width: 25%;
        padding: 20px;
    }

    .content-container {
        width: 75%;
        padding: 20px;
    }

    .card-columns {
        display: flex;
        flex-wrap: wrap;
        gap: 20px;
        justify-content: flex-start;
    }

    .card {
        flex: 0 0 30%;
        max-width: 30%;
        box-sizing: border-box;
    }

    .card-img-container {
        position: relative;
    }

    .card-img-top {
        width: 100%;
        height: auto;
        display: block;
    }

    .status-badge {
        position: absolute;
        top: 10px;
        right: 10px;
        background-color: #007bff;
        color: white;
        padding: 5px 10px;
        border-radius: 5px;
        font-size: 12px;
        font-weight: bold;
    }

    .card-body {
        padding: 15px;
        text-align: left;
    }

    .card-text {
        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis;
        max-width: 100%;
    }

    .capacity {
        font-weight: bold;
        color: #007bff;
    }
</style>

<div class="container mt-4">
    <h1 class="text-center">Les espaces disponibles</h1>

    <div class="row mt-3">
        <div class="col-md-2 filter-container">
            <h5>Filtrer la liste</h5>
            <form>
                <div class="form-group">
                    <input type="text" class="form-control" placeholder="Rechercher quelque chose..." style="height: 100%;">
                </div>
                <!-- <div>
                    <label for="filterActivities">Filtrer par Activités:</label>
                    <select multiple class="form-control" id="filterActivities">
                        <option value="Piscine">Piscine</option>
                        <option value="Tennis">Tennis</option>
                        <option value="Salle de réception">Salle de réception</option>
                        <option value="Salle de sport">Salle de sport</option>
                    </select>
                </div>
                <div class="form-group">
                    <label for="filterPrice">Filtrer par Prix:</label>
                    <select class="form-control" id="filterPrice">
                        <option value="1">Moins de 6000 Ar</option>
                        <option value="2">Entre 6000 et 8000 Ar</option>
                        <option value="3">Plus de 8000 Ar</option>
                    </select>
                </div> -->
                <br>
                <button type="submit" class="btn btn-primary">Rechercher</button>
            </form>
        </div>

        <div class="col-md-10 content-container">
            @if (pagedSpaces != null && pagedSpaces.Any())
            {
                <div class="card-columns">
                    @foreach (var space in pagedSpaces)
                    {
                        <div class="card shadow-lg rounded-3">
                            <div class="card-img-container overflow-hidden rounded-top">
                                <img src="@($"{ViewData["ImageBaseUrl"]}/{space.photos[0]}")" class="card-img-top" alt="Space Image">
                            </div>
                            <div class="card-body text-center">
                                <h5 class="card-title mb-3">@space.name</h5>
                                 <div class="demo-inline-spacing">
                                    <span class="badge rounded-pill bg-label-primary">Primary</span>
                                    <span class="badge rounded-pill bg-label-secondary">Secondary</span>
                                    <span class="badge rounded-pill bg-label-success">Success</span>
                                    <span class="badge rounded-pill bg-label-danger">Danger</span>
                                    <span class="badge rounded-pill bg-label-warning">Warning</span>
                                    <span class="badge rounded-pill bg-label-info">Info</span>
                                    <span class="badge rounded-pill bg-label-dark">Dark</span>
                                </div>
                                <a href="@Url.Action("Details", "Space", new { id = space.id })" >
                                    <button class="btn btn-info">
                                    Voir détails
                                    </button>
                                </a>
                            </div>
                        </div>
                    }
                </div>

                <div class="pagination mt-4">
                    <ul class="pagination">
                        @if (pageNumber > 1)
                        {
                            <li class="page-item">
                                <a class="page-link" href="?page=1" aria-label="First">
                                    <span aria-hidden="true">&laquo;&laquo;</span>
                                </a>
                            </li>
                            <li class="page-item">
                                <a class="page-link" href="?page=@(pageNumber - 1)" aria-label="Previous">
                                    <span aria-hidden="true">&laquo;</span>
                                </a>
                            </li>
                        }

                        @for (int i = 1; i <= totalPages; i++)
                        {
                            <li class="page-item @(i == pageNumber ? " active"
                                : "" )">
                                <a class="page-link" href="?page=@i">@i</a>
                            </li>
                        }

                        @if (pageNumber < totalPages)
                        {
                            <li class="page-item">
                                <a class="page-link" href="?page=@(pageNumber + 1)" aria-label="Next">
                                    <span aria-hidden="true">&raquo;</span>
                                </a>
                            </li>
                            <li class="page-item">
                                <a class="page-link" href="?page=@totalPages" aria-label="Last">
                                    <span aria-hidden="true">&raquo;&raquo;</span>
                                </a>
                            </li>
                        }
                    </ul>
                </div>
            }
            else
            {
                <p>Aucun espace disponible.</p>
            }
        </div>

    </div>
</div>

<!-- 
                                <button
                                    class="btn btn-primary" 
                                    data-bs-toggle="modal" 
                                    data-bs-target="#backDropModal" 
                                    data-space-name="@space.name"
                                    data-space-id="@space.id">
                                    Réserver
                                </button>
<div class="modal fade" id="backDropModal" data-bs-backdrop="static" tabindex="-1">
    <div class="modal-dialog">
         <form class="modal-content" id="reservationForm" method="POST" action="@Url.Action("CreateReservation", "Reservation")">
            <div class="modal-header">                
                <h5 class="modal-title" id="backDropModalTitle">Faire une réservation</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body">
                <input type="hidden" id="inputSpaceId" name="SpaceId">
                <div class="row g-6">
                    <div id="alertMessage"></div>
                </div>

                <div class="modal-body">
                    <input type="hidden" id="inputSpaceId" name="SpaceId">
                    <div class="row g-6">
                        <div id="alertMessage" class="text-danger"></div>
                    </div>

                    <div class="row g-6">
                        <h3 id="durationDisplay"></h3>
                    </div>

                    <div class="row g-6">
                        <div class="col mb-0">
                            <label for="startDate" class="form-label">Date de début</label>
                            <input type="datetime-local" id="startDate" name="StartDate" class="form-control">
                        </div>
                        <div class="col mb-0">
                            <label for="endDate" class="form-label">Date de fin</label>
                            <input type="datetime-local" id="endDate" name="EndDate" class="form-control">
                        </div>
                    </div>
                </div>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-outline-secondary" data-bs-dismiss="modal">Fermer</button>
                <button type="submit" id="button-valide-reservation" class="btn btn-primary" id="saveReservation">Valider la réservation</button>
            </div>
        </form>
    </div>
</div> -->

