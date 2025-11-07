/**
 * Aplica máscara de CPF no formato 000.000.000-00
 * @param {HTMLInputElement} input 
 */
function aplicarMascaraCPF(input) {
    if (!input) return;
    
    let valor = input.value.replace(/\D/g, '');
    
    if (valor.length <= 3) {
        valor = valor;
    } else if (valor.length <= 6) {
        valor = valor.replace(/(\d{3})(\d+)/, '$1.$2');
    } else if (valor.length <= 9) {
        valor = valor.replace(/(\d{3})(\d{3})(\d+)/, '$1.$2.$3');
    } else {
        valor = valor.replace(/(\d{3})(\d{3})(\d{3})(\d+)/, '$1.$2.$3-$4');
    }
    
    input.value = valor;
    console.log('CPF formatado:', valor);
}

/**
 * Formata uma string CPF
 * @param {string} cpf 
 * @returns {string}
 */
function formatarCPF(cpf) {
    if (!cpf) return '';
    
    let valor = cpf.replace(/\D/g, '');
    
    if (valor.length <= 3) {
        return valor;
    } else if (valor.length <= 6) {
        return valor.replace(/(\d{3})(\d+)/, '$1.$2');
    } else if (valor.length <= 9) {
        return valor.replace(/(\d{3})(\d{3})(\d+)/, '$1.$2.$3');
    } else {
        return valor.replace(/(\d{3})(\d{3})(\d{3})(\d+)/, '$1.$2.$3-$4');
    }
}

/**
 * Remove formatação do CPF, deixando apenas números
 * @param {string} cpfFormatado
 * @returns {string}
 */
function limparCPF(cpfFormatado) {
    return cpfFormatado ? cpfFormatado.replace(/\D/g, '') : '';
}

/**
 * Permite apenas números em um campo de input
 * @param {Event} event
 * @returns {boolean}
 */
function apenasNumeros(event) {
    const key = event.keyCode || event.which;
    
    // Permite: backspace(8), delete(46), tab(9), escape(27), enter(13)
    if ([8, 9, 27, 13, 46].indexOf(key) !== -1 ||
        // Permite: Ctrl+A, Ctrl+C, Ctrl+V, Ctrl+X
        (event.ctrlKey && [65, 67, 86, 88].indexOf(key) !== -1) ||
        // Permite: setas
        (key >= 35 && key <= 40)) {
        return true;
    }
    
    // Permite apenas números (0-9)
    if ((key < 48 || key > 57) && (key < 96 || key > 105)) {
        event.preventDefault();
        return false;
    }
    
    return true;
}

/**
 * Configura máscara de CPF em um elemento automaticamente
 * @param {string|HTMLInputElement} elemento
 */
function configurarMascaraCPF(elemento) {
    let input;
    
    if (typeof elemento === 'string') {
        input = document.getElementById(elemento);
    } else {
        input = elemento;
    }
    
    if (!input) {
        console.error('Elemento não encontrado para máscara CPF:', elemento);
        return;
    }
    
    input.removeEventListener('input', aplicarMascaraCPF);
    input.removeEventListener('keypress', apenasNumeros);
    
    input.addEventListener('input', function() {
        aplicarMascaraCPF(this);
    });
    
    input.addEventListener('keypress', apenasNumeros);
    
    input.setAttribute('maxlength', '14');
    input.setAttribute('placeholder', '000.000.000-00');
    
    if (input.value && input.value.trim() !== '') {
        aplicarMascaraCPF(input);
    }
    
    console.log('Máscara CPF configurada para:', input.id || input.name);
}

function inicializarMascaras() {
    console.log('Inicializando máscaras automáticas...');
    
    const camposCPF = document.querySelectorAll('input[id*="cpf"], input[name*="CPF"], input[data-mask="cpf"]');
    camposCPF.forEach(function(campo) {
        console.log('Configurando máscara CPF para:', campo.id || campo.name);
        configurarMascaraCPF(campo);
    });
    
    const camposTelefone = document.querySelectorAll('input[id*="telefone"], input[name*="Telefone"], input[data-mask="telefone"]');
    camposTelefone.forEach(function(campo) {
        campo.addEventListener('input', function() {
            aplicarMascaraTelefone(this);
        });
        campo.addEventListener('keypress', apenasNumeros);
        campo.setAttribute('maxlength', '15');
        campo.setAttribute('placeholder', '(00) 00000-0000');
        console.log('Máscara de telefone configurada para:', campo.id || campo.name);
    });
    
    const camposCEP = document.querySelectorAll('input[id*="cep"], input[name*="CEP"], input[data-mask="cep"]');
    camposCEP.forEach(function(campo) {
        campo.addEventListener('input', function() {
            aplicarMascaraCEP(this);
        });
        campo.addEventListener('keypress', apenasNumeros);
        campo.setAttribute('maxlength', '9');
        campo.setAttribute('placeholder', '00000-000');
        console.log('Máscara de CEP configurada para:', campo.id || campo.name);
    });
    
    console.log('Máscaras automáticas inicializadas!');
}

document.addEventListener('DOMContentLoaded', inicializarMascaras);

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', inicializarMascaras);
} else {
    inicializarMascaras();
}