---
name: "🐞 Reportar Bug"
about: Relato de comportamento incorreto, falha ou erro na aplicação
title: "bug: "
labels: ["bug"]
assignees: ""
---

# **🐞 Reporte de Bug**

---

### **🧠 O que acontece?**

Descreva o problema de forma objetiva.  
Exemplo: Ao confirmar o e-mail, o sistema retorna erro 500 mesmo com código válido.

---

### **🎯 O que você esperava que acontecesse?**

Explique qual era o comportamento esperado.  
Exemplo: O sistema deveria confirmar o e-mail e retornar `204 No Content`.

---

### **📋 Passos para reproduzir**

1. Descreva passo a passo o que foi feito  
2. Informe o endpoint, ação ou tela afetada  
3. Inclua payloads, parâmetros ou contexto, se aplicável  

Exemplo:
1. Cadastrar novo usuário  
2. Receber e-mail de confirmação  
3. Enviar código válido para `/api/v1/users/confirm-email`  
4. Ocorre erro 500 no response  

---

### **🧾 Algum log, saída de erro ou detalhe adicional?**

Adicione logs, stacktrace, ou qualquer informação útil.  
Exemplo:

```
System.NullReferenceException: Object reference not set to an instance of an object.

```

- **Ambiente:** (ex.: dev, staging, produção)  
- **Versão:** (commit, tag, ou branch)  
- **Data/Hora:**  
- **Usuário afetado (opcional):**  

---

### **📸 Evidências**

Anexe capturas de tela, logs, ou respostas da API que comprovem o problema.

---

### **📍 Impacto**

Descreva o impacto do bug:  
- [ ] Alto — impede uso da funcionalidade principal  
- [ ] Médio — afeta funcionalidade secundária  
- [ ] Baixo — comportamento inesperado sem impacto crítico  

---

### **✅ Critério de correção**

Defina o que validará a correção do bug.  
Exemplo: O endpoint `/api/v1/users/confirm-email` deve retornar `204` e atualizar o status do usuário.
