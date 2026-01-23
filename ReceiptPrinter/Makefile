VENV = .venv

install:
	python3 -m venv $(VENV)
	$(VENV)/bin/pip install --upgrade pip
	$(VENV)/bin/pip install python-escpos pyusb pytest pylint requests fastapi uvicorn

test-print:
	LD_LIBRARY_PATH=$$HOME/.nix-profile/lib:$$LD_LIBRARY_PATH $(VENV)/bin/python test_print.py

lint:
	$(VENV)/bin/pylint --disable=missing-module-docstring,missing-function-docstring,missing-class-docstring *.py

check-warnings:
	$(VENV)/bin/pytest -W error

run-server:
	$(VENV)/bin/uvicorn main:app --reload --host 0.0.0.0 --port 8000

clean:
	rm -rf $(VENV)

reinstall: clean install
