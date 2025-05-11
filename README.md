
# Add to a World

Note: When simulating your world in the editor, a translation must be manually selected before using. This is to reduce load times on startup.

# Add to an Avatar


# Change the Size / Resolution

Changing the size and/or resolution of the window can only be done via the Unity editor.
To do this, there are a few numerical values that must be set. Start in the GameObject `bible_ui` and update the 3D scale (the default is 0.01).
Then, to set the dimensions, set the `RectTransform.width` and `RectTransform.height`, and also update the `BoxCollider.size` to match.
Finally, select the GameObject `bible_ui > Panel > Main Window Container` and update the height to match the bottom of the canvas.


# Translations

You can have as many translations as you want, but they must be preinstalled on the Bible Reader attached to the world or avatar. They are not streamed in or fetched from an online source when loading into the world. The reason for this is so that no user needs to enable `Allow Untrusted URLs`; translations will be included with the world/avatar when it loads. Translations are fairly large (for text files, around ~4MB), so having dozens and dozens of translations will add up the file size.

## Formatting a translation

Each translation comes as a single `.txt` file with two sections: a <b>HEADER</b> section and a <b>CONTENT</b> section. The formatting of this file is essential to ensure the text is displayed properly. First of all, the file must be in LF, UTF-8 format. It can be located anywhere in the project.

The Header section contains the names of each book as well as the number of chapters in each book. Each line represents one book, in the format `<BOOK_NAME>,<CHAPTER_COUNT>` , e.g. `GENESIS,50` . An empty line marks the end of the Header section.

The Content section contains the content of the translation. Each line represents one verse, in the format `<VERSE_ADDRESS> <VERSE_CONTENT>` where `VERSE_ADDRESS` is a string of nine digits denoting the book, chapter, and verse numbers, and `VERSE_CONTENT` is the text itself. The verse content may contain formatting tokens that TextMeshPro can parse, such as `<i>` or `<b>`.
An empty string of zeroes `000000000` marks the end of the Content section. This must be the last line in the file.

If you obtain your translation from https://bolls.life/api/, you can also use my custom formatting tool to quickly convert this data into the proper format. If your translation comes from any other source, you will need to format the data manually (ideally, using a script).

## Adding the translation to the reader UI

To actually have the translation become available in the reader, you'll need to add a new entry for it so users can select it.

1. Go into the `translation_window` prefab and add a new `translation_button` prefab (or copy an existing one).
2. Set the `Title` field to the full name of the translation.
3. Set the `Abbr` field to the abbreviated name of the translation.
4. Set the `Content` field to the new translation text file.
5. (Optional) If your translation does not use latin characters, set the fields `Button_font_prefab` and `Reader_font_prefab` to the appropriate prefab GameObjects (see [this section](#adding-fonts-for-non-latin-languages) for how to configure those).
6. In the `label` object, set the text and/or font.
7. In `bible_ui` prefab, make sure that the new button's `Host` field is set to `bible_ui (Bible Host)`.
8. (Optional) To set the translation as the default, go to the `bible_ui` prefab > `BibleHost` script > `trans_default` field, click the radio button to select a new default.

## Adding fonts for non-Latin languages

For non-Latin languages there are a few more steps you'll need to do to make sure your text displays properly in the reader.

1. Obtain a suitable font to display the text and import it into your project. [Google Fonts Noto](https://fonts.google.com/noto) is a free, open source for many languages.
2. Configure the font.
	1. Select the font in the Unity Project browser and use Shift+Ctrl+F12 to create a new Font Asset for TextMeshPro.
	2. Select the Font Asset and click the `Update Atlas Texture` button. This will open a new window.
	3. (Optional) Set `Packing Method` to Optimum.
	4. Set `Atlas Resolution` to `2048x2048`.
	5. In the `Character Set` field, select `Characters from File`.
	6. In the `Character File` field, select your new translation file.
	7. Click `Generate Font Atlas` and wait for it to finish.
	8. Click `Save`.
	9. (Optional) If the resulting text appears blurry or poor quality, raise the `Atlas Resolution` to a higher value and repeat these steps.
2. Create a Prefab Variant of the prefab `label_latin` and set its font to the new Font Asset. This will be used on book buttons in the book selection window.
3. Create a Prefab Variant of the prefab `reader_group_en` and set the fonts of the objects `book` and `content` to the new Font Asset. This will be used in the reader section. (Note: the text might appear broken/squashed in the prefab, but this will be corrected in the game.)
