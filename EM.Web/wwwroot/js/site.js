// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// ===== MÓDULO DE MÁSCARAS =====

/**
 * Aplica máscara de CPF no formato 000.000.000-00
 * @param {HTMLInputElement} input - Elemento input onde aplicar a máscara
 */
function aplicarMascaraCPF(input) {
    if (!input) return;
    
    let valor = input.value.replace(/\D/g, ''); // Remove tudo que não é dígito
    
    // Aplica a máscara progressivamente
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
 * Formata uma string CPF (uso estático)
 * @param {string} cpf - String do CPF a ser formatada
 * @returns {string} CPF formatado
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
 * @param {string} cpfFormatado - CPF formatado
 * @returns {string} CPF apenas com números
 */
function limparCPF(cpfFormatado) {
    return cpfFormatado ? cpfFormatado.replace(/\D/g, '') : '';
}

/**
 * Permite apenas números em um campo de input
 * @param {Event} event - Evento do teclado
 * @returns {boolean} True se a tecla é permitida, false caso contrário
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
 * @param {string|HTMLInputElement} elemento - ID do elemento ou elemento HTML
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
    
    // Remove listeners existentes para evitar duplicação
    input.removeEventListener('input', aplicarMascaraCPF);
    input.removeEventListener('keypress', apenasNumeros);
    
    // Adiciona eventos
    input.addEventListener('input', function() {
        aplicarMascaraCPF(this);
    });
    
    input.addEventListener('keypress', apenasNumeros);
    
    // Configura atributos
    input.setAttribute('maxlength', '14');
    input.setAttribute('placeholder', '000.000.000-00');
    
    // Aplica máscara se já houver valor
    if (input.value && input.value.trim() !== '') {
        aplicarMascaraCPF(input);
    }
    
    console.log('Máscara CPF configurada para:', input.id || input.name);
}

// ===== OUTRAS MÁSCARAS ÚTEIS =====

/**
 * Aplica máscara de telefone (00) 00000-0000
 * @param {HTMLInputElement} input - Elemento input
 */
function aplicarMascaraTelefone(input) {
    if (!input) return;
    
    let valor = input.value.replace(/\D/g, '');
    
    if (valor.length <= 2) {
        valor = valor;
    } else if (valor.length <= 7) {
        valor = valor.replace(/(\d{2})(\d+)/, '($1) $2');
    } else if (valor.length <= 10) {
        valor = valor.replace(/(\d{2})(\d{4})(\d+)/, '($1) $2-$3');
    } else {
        valor = valor.replace(/(\d{2})(\d{5})(\d+)/, '($1) $2-$3');
    }
    
    input.value = valor;
}

/**
 * Aplica máscara de CEP 00000-000
 * @param {HTMLInputElement} input - Elemento input
 */
function aplicarMascaraCEP(input) {
    if (!input) return;
    
    let valor = input.value.replace(/\D/g, '');
    
    if (valor.length <= 5) {
        valor = valor;
    } else {
        valor = valor.replace(/(\d{5})(\d+)/, '$1-$2');
    }
    
    input.value = valor;
}

// ===== INICIALIZAÇÃO AUTOMÁTICA =====

/**
 * Inicializa todas as máscaras automaticamente quando a página carrega
 */
function inicializarMascaras() {
    console.log('🚀 Inicializando máscaras automáticas...');
    
    // Configura automaticamente todos os campos CPF
    const camposCPF = document.querySelectorAll('input[id*="cpf"], input[name*="CPF"], input[data-mask="cpf"]');
    camposCPF.forEach(function(campo) {
        console.log('🔧 Configurando máscara CPF para:', campo.id || campo.name);
        configurarMascaraCPF(campo);
    });
    
    // Configura automaticamente campos de telefone
    const camposTelefone = document.querySelectorAll('input[id*="telefone"], input[name*="Telefone"], input[data-mask="telefone"]');
    camposTelefone.forEach(function(campo) {
        campo.addEventListener('input', function() {
            aplicarMascaraTelefone(this);
        });
        campo.addEventListener('keypress', apenasNumeros);
        console.log('📞 Máscara de telefone configurada para:', campo.id || campo.name);
    });
    
    // Configura automaticamente campos de CEP
    const camposCEP = document.querySelectorAll('input[id*="cep"], input[name*="CEP"], input[data-mask="cep"]');
    camposCEP.forEach(function(campo) {
        campo.addEventListener('input', function() {
            aplicarMascaraCEP(this);
        });
        campo.addEventListener('keypress', apenasNumeros);
        campo.setAttribute('maxlength', '9');
        campo.setAttribute('placeholder', '00000-000');
        console.log('📮 Máscara de CEP configurada para:', campo.id || campo.name);
    });
    
    console.log('✅ Máscaras automáticas inicializadas!');
}

// ===== AUTO-EXECUÇÃO =====

// Executa quando o DOM estiver pronto
document.addEventListener('DOMContentLoaded', inicializarMascaras);

// Fallback para garantir que funcione mesmo em carregamentos tardios
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', inicializarMascaras);
} else {
    inicializarMascaras();
}
