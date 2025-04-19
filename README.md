# MVVMatic

MVVMatic is een codegeneratie-tool voor WPF-projecten die gebruikmaken van het MVVM-patroon. Het doel is om repetitief werk te automatiseren, zoals het opzetten van Views, ViewModels, Models en Services. Zo ontwikkel je sneller en consistenter binnen een goed gestructureerde MVVM-architectuur.

## 🔧 Werken met Templates

MVVMatic maakt gebruik van **aanpasbare templates** om de gegenereerde code consistent en onderhoudbaar te houden.  
Deze templates staan in de map `Templates/` en bevatten placeholders die automatisch worden ingevuld op basis van gebruikersinput (zoals module-naam of namespace).

Voorbeeld van een placeholder: `{{ModuleName}}` → wordt vervangen door `Productbeheer`.

## 🚀 Functionaliteiten

- Genereert automatisch:
  - ViewModel-classes
  - Views met gekoppelde DataTemplates
  - Models met basis-structuur
  - Services (optioneel)
- Ondersteuning voor generieke ViewModels
- Structuur volgens best practices
- Eenvoudig uitbreidbaar en aanpasbaar

## 📦 Installatie

1. Clone deze repository:
   ```bash
   git clone https://github.com/knoopsr/MVVMatic.git
