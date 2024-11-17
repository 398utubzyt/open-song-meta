# Open Song Meta

Open Song Meta (OPSM) is a binary file format to describe information about a specific song. This includes:
- Song title
- Song artist(s)
- Song album
- Tempo track
- Time signatures
- Key signatures

This repository is the home to the API which can read/write OPSM files directly, and convert DAW projects to the OPSM format.

> [!WARNING]
>
> The format is currently considered _unstable_, which means that
> changes to the format can and will be made without notice and
> without support for older "unstable" versions.
> This is to ensure the project is finalized faster.
>
> Check out the [to-do list below](#whats-left-to-do) to see what you can do to help!

## What's left to do?

Below is a checklist of everything that needs to get done before the format is finalized:

- [ ] Streamlined support for both old and current versions
- [ ] Switch to a modular format, allowing data to be written "out of order"
  - [ ] Add support for custom data "modules," making this an extensible format
- [ ] Less reliance on try-catch and exceptions (catch errors before-hand)
- [ ] FL Studio importer
- [ ] Write documentation

This list will likely change as the scope of the project changes.
