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

password.onchange = (ev) => {
	if (!/\p{Nd}/u.test(password.value)) {
		password.setCustomValidity("A senha deve ter ao menos 1 caractere numérico.")
	}
	else if (!/\p{Lu}/u.test(password.value)) {
		password.setCustomValidity("A senha deve ter ao menos 1 caractere maiúsculo.")
	}
	else if (!/\p{Ll}/u.test(password.value)) {
		password.setCustomValidity("A senha deve ter ao menos 1 caractere minúsculo.")
	}
	else if (!/[^\p{L}\p{N}]/u.test(password.value)) {
		password.setCustomValidity("A senha deve ter ao menos 1 caractere não alfanumérico.")
	}
	else {
		password.setCustomValidity("")
	}
}

form.onsubmit = async (ev) => {
	ev.preventDefault()

	let data = new FormData(form)
	
	data = Object.fromEntries(data.entries())
	data = JSON.stringify(data)
	
	let response = await fetch(form.action, {
		method: "post",
		body: data,
		headers: { "Content-Type": "application/json", "Accept": "application/json" }
	})

	let { id } = await response.json()
	
	if (response.status === 201) {
		document.cookie = `login_id=${id}; path=/; max-age=3600`;
		document.location.assign("/2fa")
	}
	else if (response.status === 400) {
		let errorDialog = document.getElementById("signup-error")
		
		errorDialog.show()
	}
}
