# GitHub Copilot Instructions for Lord of the Rims - The Third Age (Continued)

## Mod Overview and Purpose
"Lord of the Rims - The Third Age (Continued)" is an unofficial port of the original mod by erdelf and Jecrell. It aims to enhance the RimWorld gaming experience by creating an immersive medieval world. The mod strips away all items, incidents, and technologies that extend beyond the medieval era, while introducing new and rebalanced content to offer a compelling medieval-themed playthrough.

## Key Features and Systems
- **Technology Limitation**: Option to disable advanced technology limits, similar to "Lord of the Rims - Unlimited."
- **Combat and Effects Integration**:
  - Support for Combat Extended (though no longer updated).
  - Support for Simple FX: Smoke.
- **New Resources and Crafting**:
  - Introduces a blacksmith forge for forging steel.
  - Adds iron and mithril as resources, making iron the primary mineable resource.
- **Medieval Storage Solutions**: 
  - Medieval pantries and salt barrels for food storage.
- **New Scenarios**: Features scenarios inspired by "Lord of the Rings" races.
- ****: Instructions to avoid using this mod alongside "LOTR Unlimited."

## Coding Patterns and Conventions
- **Naming Conventions**: Use PascalCase for class names, methods, and public members. Variables use camelCase.
- **File Organization**: Group related classes and functionalities into specific files, each focusing on a module or feature.
- **Mod Clarity**: Comments and XML documentation are encouraged to clarify complex logic and intended use.

## XML Integration
- Utilize XML for defining game data and settings, such as items, resources, and scenarios.
- Employ XML extensions for compatibility with other mods and updates (e.g., XML patching).
- Maintain well-structured XML files for ease of use and modification.

## Harmony Patching
- Use Harmony library to apply patches that alter or extend the core game functionality without direct modification of game code.
- Create patches in static classes, such as `RemoveModernStuffHarmony`, to manage customized updates while avoiding conflict with original game mechanics.
- Focus patches on relevant methods to implement game logic changes, like removing modern elements or adding medieval features.

## Suggestions for Copilot
- Generate boilerplate code for new classes and methods following the outlined naming conventions and patterns.
- Suggest XML schema structures for defining new game content, ensuring compatibility with RimWorld's data format.
- Provide Harmony patch templates to assist in creating non-destructive game adjustments.
- Offer code snippets for common functionalities, such as resource management, graphics handling, and medieval theming, to speed up development.
- Ensure all generated code includes appropriate error-checking and logging features.

## Additional Notes
- **Mod Order**: Place "The Third Age" high on the mod order list. Recommended alongside mods like HugsLib.
- **Community and Support**: Engage with the RimWorld community through dedicated channels for discussion and feedback, though primary support is not provided.
- **Development Goals**: Continually update and fix bugs to improve the mod, and aim to introduce new content, such as adding a certain iconic ring.

These instructions provide a foundational guide for using GitHub Copilot effectively in the development of "Lord of the Rims - The Third Age (Continued)." Following these guidelines ensures the mod remains consistent, functional, and immersive for players seeking a medieval experience in RimWorld.
