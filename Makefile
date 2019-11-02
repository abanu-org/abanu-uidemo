CUR_DIR = $(CURDIR)

OUT := ../abanu/os/App.GuiDemo.bin
NET := bin/App.GuiDemo.exe

# virtual targets

all: net out

out: $(OUT)

net: $(NET)

.PHONY: all net out

# file targets

$(OUT): $(NET)
	cd $(CUR_DIR)
	../abanu/abctl build external $(shell ../abanu/abctl abspath $(abspath bin/App.GuiDemo.exe))

$(NET):
	cd $(CUR_DIR)
	(cd src && ../../abanu/abctl tool msbuild /p:Configuration=Debug /m -Verbosity:minimal)
