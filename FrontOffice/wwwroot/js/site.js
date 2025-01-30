function parseJwt(token) {
    const base64Url = token.split('.')[1]; // On récupère la deuxième partie du token JWT
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/'); // Décodage du base64
    const jsonPayload = decodeURIComponent(
        atob(base64)
            .split('')
            .map((c) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
            .join('')
    );
    return JSON.parse(jsonPayload); // On retourne les données décodées
}

function updateLoginButton(token) {
    try {
        const userInfo = parseJwt(token); // Récupère les informations de l'utilisateur à partir du token
        console.log("--------------userInfo--------------------", userInfo);
        const loginButton = document.getElementById("loginButton");
        const logoutButton = document.getElementById("logoutBuutton");

        if (userInfo) {
            // Mettre à jour le texte du bouton de connexion avec le nom et le rôle de l'utilisateur
            loginButton.innerHTML = `<span>${userInfo.name} (${userInfo.role})</span>`;
            loginButton.classList.add("disabled"); // Désactive le bouton de connexion
            loginButton.removeAttribute("data-bs-toggle");
            loginButton.removeAttribute("data-bs-target");

            loginButton.style.display = "none";
            logoutButton.style.display = "inline-block";
        }
    } catch (error) {
        console.error("Erreur lors du décodage du token JWT :", error);
    }
}

document.addEventListener('DOMContentLoaded', function () {
    const token = localStorage.getItem("jwt");
    const isAuthenticated = token ? true : false;
    console.log(isAuthenticated);
    const loginSection = document.getElementById("loginSection");
    const userSection = document.getElementById("userSection");

    const loginButton = document.getElementById("loginButton");
    const signupButton = document.getElementById("signupButton");
    const userInfoButton = document.getElementById("userInfoButton");
    const logoutButton = document.getElementById("logoutButton");
    const myFavorite = document.getElementById("my-favorite");
    const logoutModal = new bootstrap.Modal(document.getElementById('logoutModal'));
    const confirmLogoutButton = document.getElementById('confirmLogoutButton');

    if (isAuthenticated) {
        const userInfo = parseJwt(token);
        console.log(userInfo);
        // userInfoButton.innerHTML = `${userInfo.Name} (${userInfo.Role})`;
        userInfoButton.innerHTML = `${userInfo.Name}`;

        loginSection.style.display = "none";
        userSection.style.display = "flex";
        myFavorite.style.display = "block";
    } else {
        loginSection.style.display = "flex";
        userSection.style.display = "none";
        myFavorite.style.display = "none";
    }

    // Ajouter un écouteur d'événement au bouton de déconnexion
    logoutButton.addEventListener('click', function () {
        // Ouvrir le modal de déconnexion
        logoutModal.show();
    });

    // Ajouter un écouteur d'événement au bouton de confirmation de déconnexion
    confirmLogoutButton.addEventListener('click', async function () {
        try {
            // Fermer le modal après la confirmation
            logoutModal.hide();
            const response = await fetch('/Auth/Logout', {
                method: 'GET', // La méthode correspond à celle de votre API
                credentials: 'include', // Inclut les cookies dans la requête
            });
            console.log(response);
            if (response.ok) {
                localStorage.removeItem("jwt"); // Supprimer le JWT du localStorage
                console.log("Déconnexion réussie !");
                loginSection.style.display = "flex";
                userSection.style.display = "none";
                myFavorite.style.display = "none";
                // Redirection vers la page d'accueil
                window.location.href = '/';
            } else {
                // Gérer les erreurs éventuelles
                console.error("Erreur lors de la déconnexion :", response.statusText);
            }
        } catch (error) {
            console.error("Erreur réseau :", error);
        }
    });

    // Gestion de la soumission du formulaire de connexion
    document.getElementById("loginForm").addEventListener("submit", async function (e) {
        e.preventDefault();
        const username = document.getElementById("username").value;
        const password = document.getElementById("password").value;

        try {
            // Requête à l'API pour l'authentification
            const response = await fetch("/Auth/Authenticate", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({ username, password })
            });

            const result = await response.json();
            // Stocker le token dans localStorage
            const token = result.token; // Récupérer le token de la réponse
            localStorage.setItem('jwt', token);

            if (token) {

                location.reload();
                console.log("Authentification réussie!");

                // Stocker le token dans les cookies
                document.cookie = `jwt=${token}; path=/;`;

                // Mettre à jour l'interface avec les informations utilisateur
                updateLoginButton(token);

                // Fermer le modal de connexion si nécessaire
                const loginModal = document.getElementById("loginModal");
                if (loginModal) {
                    const bootstrapModal = bootstrap.Modal.getInstance(loginModal);
                    bootstrapModal?.hide();
                }
            } else {
                console.log("Authentification échouée!");
                // Afficher un message d'erreur
                document.getElementById("loginError").textContent = "Identifiants incorrects.";
                document.getElementById("loginError").style.display = "block";
            }
        } catch (error) {
            console.error("Erreur lors de la connexion :", error);
            document.getElementById("loginError").textContent = "Une erreur est survenue.";
            document.getElementById("loginError").style.display = "block";
        }
    });

    // Gestion de la déconnexion
    logoutButton.addEventListener('click', function () {
        // Supprimer le token des cookies
        document.cookie = "jwt=; path=/; expires=Thu, 01 Jan 1970 00:00:00 GMT";

        // Mettre à jour l'interface pour masquer le bouton de déconnexion et afficher celui de connexion
        loginButton.style.display = "inline-block";
        signupButton.style.display = "inline-block";
        logoutButton.style.display = "none";
    });

    document.getElementById("signupForm").addEventListener("submit", async function (e) {
        e.preventDefault();

        const submitSignupButton = document.getElementById("submit_signup_button");
        submitSignupButton.textContent = " Loading . . .";
        const name = document.getElementById("singup_fullName").value;
        const username = document.getElementById("singup_email").value;
        const phone = document.getElementById("singup_phone").value;
        const password = document.getElementById("singup_password").value;
        const password_retape = document.getElementById("singup_password_retape").value;

        const signupErrorElement = document.getElementById("signupError");

        // Vérification que les mots de passe correspondent
        if (password !== password_retape) {
            signupErrorElement.textContent = "Les mots de passe ne correspondent pas.";
            signupErrorElement.style.display = "block";
            submitSignupButton.textContent = " S'inscrire";
            return; // Empêcher la soumission du formulaire
        }

        try {
            // Requête à l'API pour l'inscription
            const response = await fetch("/Auth/Signup", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({ username, password, phone, name })
            });

            const result = await response.json();
            console.log(result);

            if (response.ok) {
                const token = result.token; // Récupérer le token de la réponse
                console.log(token);
                localStorage.setItem('jwt', token);

                // Stocker le token dans les cookies
                document.cookie = `jwt=${token}; path=/;`;

                // Mettre à jour l'interface avec les informations utilisateur
                updateLoginButton(token);

                // Fermer le modal de connexion si nécessaire
                const signupModal = document.getElementById("signupModal");
                if (signupModal) {
                    const bootstrapModal = bootstrap.Modal.getInstance(signupModal);
                    bootstrapModal?.hide();
                }

                location.reload();
                console.log("Inscription réussie!");

            } else {
                signupErrorElement.textContent = result.message || "Inscription échouée. Essayez à nouveau.";
                signupErrorElement.style.display = "block";
            }
        } catch (error) {
            console.error("Erreur lors de l'inscription :", error);
            signupErrorElement.textContent = "Une erreur est survenue lors de l'inscription.";
            signupErrorElement.style.display = "block";
        }
        finally {
            submitSignupButton.textContent = " S'inscrire";
        }
    });

});
