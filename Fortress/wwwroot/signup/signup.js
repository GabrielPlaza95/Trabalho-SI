let form = document.getElementById("signup-form")

let password = document.getElementById("signup-password")
let passwordConfirm = document.getElementById("signup-confirm-password")
let passwordShow = document.getElementById("signup-show-password")

passwordShow.onchange = (ev) => {
	password.type = passwordShow.checked ? "text" : "password"
	passwordConfirm.type = passwordShow.checked ? "text" : "password"
}

passwordConfirm.onchange = (ev) => {
	passwordConfirm.setCustomValidity(
		passwordConfirm.value === password.value
		? ""
		: "As senhas digitadas diferem. Tente novamente."
	)
}

form.onsubmit = async (ev) => {
	ev.preventDefault()

	let response = await fetch(form.action, {
		method: "post",
		body: new FormData(form),
		headers: { "Content-Type": "application/json" }
	})

	let id = await response.text()
	
	if (response.status === 200) {
		document.location.assign("/2fa")
	}
	else if (response.status === 400) {
		
	}
}
