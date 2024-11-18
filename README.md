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

## How can Open Song Meta be used?

This project started as a tool for rhythm game charters.
The idea is that the OPSM tool will convert data from DAW project files into tempo changes and timing points to be used in the chart editor.
As a result, artists and charters don't have to carry the burden of making sure everything is in-sync.

> [!NOTE]
>
> The OPSM tool does ***NOT*** convert DAW files to rhythm game charts. It only converts DAW files to the OPSM format.

With the addition of the OPSM API, OPSM files can also be used to assist syncing background elements to the music.
Just ask the artist to send their project as an OPSM file—the rest is easy!

## What's left to do?

Below is a checklist of everything that needs to get done before the format is finalized:

- [ ] Streamlined support for both old and current versions
- [x] Switch to a modular format, allowing data to be written "out of order"
  - [x] Add support for custom data "modules," making this an extensible format
- [ ] Less reliance on try-catch and exceptions (catch errors before-hand)
- [ ] Importers for other DAWs
  - [ ] FL Studio
  - [ ] Ableton
  - [ ] REAPER
- [ ] Write documentation
- [ ] BPM Easing

This list will likely change as the scope of the project changes.
