let form = document.getElementById("2fa-form")
let setupName = document.getElementById("2fa-setup-name")
let setupKey = document.getElementById("2fa-setup-key")

function getCookie(name) {
  const value = `; ${document.cookie}`
  const parts = value.split(`; ${name}=`)
  if (parts.length === 2) return parts.pop().split(";").shift()
}

window.onload = async (ev) => {
	let UserId = getCookie("login_id")
	
	let response = await fetch("/userauth/otp/setup?" + new URLSearchParams({ UserId }), {
		method: "get",
		headers: { "Content-Type": "application/json", "Accept": "application/json" }
	})
	
	let { accountName, key } = await response.json()
	
	setupName.innerText = `Nome: ${accountName}`
	setupKey.innerText = `Chave: ${key}`
}

form.onsubmit = async (ev) => {
	ev.preventDefault()

	let data = new FormData(form)

	data = Object.fromEntries(data.entries())
	
	data.userId = getCookie("login_id")
	
	data = JSON.stringify(data)

	let response = await fetch(form.action, {
		method: "patch",
		body: data,
		headers: { "Content-Type": "application/json", "Accept": "application/json" }
	})
	
	let status = response.status
	let bearerToken = await response.text()
	
	if (response.status === 200) {
		document.location.assign("/home")
	}
	else if (response.status === 400 || response.status === 401) {
		let errorDialog = document.getElementById("2fa-error")
		
		errorDialog.show()
	}
}
