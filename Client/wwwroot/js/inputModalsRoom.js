window.showCreateRoomModal = async function () {
    const result = await Swal.fire({
        title: 'Criar Sala',
        html: `
            <div style="text-align: center;">
                <input id="roomName" class="swal2-input" placeholder="Nome da sala" style="width: 80%; margin: 10px auto; display: block;">
                <label style="display: flex; justify-content: center; align-items: center; margin: 15px 0; font-weight: bold;">
                    <input id="isPublic" type="checkbox" checked style="margin-right: 8px;"> Sala Pública
                </label>
                <input id="password" type="password" class="swal2-input" placeholder="Senha" style="width: 80%; margin: 10px auto; display: none;">
            </div>
        `,
        focusConfirm: false,
        didOpen: () => {
            const isPublicCheckbox = document.getElementById('isPublic');
            const passwordInput = document.getElementById('password');

            isPublicCheckbox.addEventListener('change', () => {
                passwordInput.style.display = isPublicCheckbox.checked ? 'none' : 'block';
            });
        },
        preConfirm: () => {
            const roomName = document.getElementById('roomName').value;
            const isPublic = document.getElementById('isPublic').checked;
            const password = document.getElementById('password').value;

            if (!roomName) {
                Swal.showValidationMessage('O nome da sala é obrigatório');
                return false;
            }

            return {
                roomName: roomName,
                password: isPublic ? '' : password
            };
        },
        showCancelButton: true
    });

    return result.isConfirmed ? result.value : null;
};

window.validateModalRoom = async function () {
    const result = await Swal.fire({
        title: 'Validar sala',
        html: '<input id="password" type="password" class="swal2-input" placeholder="Senha">',
        focusConfirm: false,
        preConfirm: () => {
            const password = document.getElementById('password').value;
            if (!password) {
                Swal.showValidationMessage('Por favor, insira a senha');
                return false;
            }
            return password;
        },
        showCancelButton: true
    });

    return result.isConfirmed ? result.value : null;
};