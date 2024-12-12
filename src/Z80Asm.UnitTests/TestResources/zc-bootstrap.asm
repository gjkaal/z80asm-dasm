; Bootstrap code for ZC computer
; Version 1.0
; This is the bootstrap code for the ZC-80 computer. It contains the
; initialization code, watchdog code and the interrupt service routines
; for INT and NMI entry points.

SYS_RESET:      EQU 0x0000
INT_VECTOR:     EQU 0038h
NMI_VECTOR:     EQU 0066h
HARDWARE_BASE:  EQU 0x0EFF
MAIN:           EQU 0xF000

; output ports, max 256 bytes (high byte is not used)
IO_PORTS:       EQU 0x00

; The second address is used for retrieving status information and configuration
SERIAL_MONITOR: EQU IO_PORTS                    ; Serial port for monitor and keyboard
SERIAL_MONITOR_DATA: EQU SERIAL_MONITOR         ; Serial port data register
SERIAL_MONITOR_OUTPUT: EQU SERIAL_MONITOR + 1   ; Serial port output register
SERIAL_MONITOR_STATUS: EQU SERIAL_MONITOR + 2   ; Serial port status register   

SERIAL1_BASE:   EQU IO_PORTS + 3        ; Serial port 1 for programs
SERIAL1_DATA:   EQU SERIAL1_BASE        ; Serial port 1 data register
SERIAL1_OUTPUT: EQU SERIAL1_BASE + 1    ; Serial port 1 output register
SERIAL1_STATUS: EQU SERIAL1_BASE + 2    ; Serial port 1 status register

SERIAL2_BASE:   EQU IO_PORTS + 6    ; Serial port 2 for programs
SERIAL2_DATA:   EQU SERIAL2_BASE    ; Serial port 2 data register
SERIAL2_OUTPUT: EQU SERIAL2_BASE + 1; Serial port 2 output register
SERIAL2_STATUS: EQU SERIAL2_BASE + 2; Serial port 2 status register

MEM_SELECT:     EQU IO_PORTS + 9    ; Memory bank select register
WATCHDOG:       EQU IO_PORTS + 10   ; Watchdog timer
INTERRUPT_REG:  EQU IO_PORTS + 11   ; Interrupt register

; Hardware flags can be used on the board to hardwire behavior
HARDWARE_FLAGS:  EQU IO_PORTS + 12   ; Hardware flags register
HARDWARE_FLAGS2: EQU IO_PORTS + 13  ; Hardware flags register 2
HARDWARE_FLAGS3: EQU IO_PORTS + 14  ; Hardware flags register 3
HARDWARE_FLAGS4: EQU IO_PORTS + 15  ; Hardware flags register 4

RAM_STATUS:     EQU IO_PORTS + 16   ; RAM status register

; Interupt handler for mode 1

; When Mode 1 is selected by the programmer, the CPU responds to an interrupt by executing 
; a restart at address 0038h. As a result, the response is identical to that of a nonmaskable 
; interrupt except that the call location is 0038h instead of 0066h. The number of cycles 
; required to complete the restart instruction is two more than normal due to the two 
; added wait states

ORG SYS_RESET  
; Max 38 bytes for startup code
    JP startup      ; Jump to startup code (3)
    NOP             ; Padding (1)

ORG INT_VECTOR
INT_HANDLER:
    ; Save registers
    PUSH AF
    PUSH BC
    PUSH DE
    PUSH HL
    ; Call the interrupt service routine
    CALL interrupt
    ; Restore registers
    POP HL
    POP DE
    POP BC
    POP AF
    ; Return from interrupt
    RETI

ORG NMI_VECTOR
NMI_HANDLER:
    ; Save registers
    PUSH AF
    PUSH BC
    PUSH DE
    PUSH HL
    ; Call the non-maskable interrupt service routine
    CALL watchdog
    ; Restore registers
    POP HL
    POP DE
    POP BC
    POP AF
    ; Return from interrupt
    RETI

startup:
; Check available RAM memory
; in banks 0x1000 to 0x7FFF
; and set the stack pointer

; Set the system stack pointer
; to the top of the first bank
; of RAM memory

; Initialize the watchdog timer
        call watchdog_kick
; Check system RAM memory
        ld HL, 0x1000
        call check_ram
        ; If error, halt the system
        cp A, 0
        jr Z, startup1
        ld IX, check_ram
        jp system_halt
startup1:
        call watchdog_kick
; Set memory bank 0
        ld IX, mem_select
        ld (IX), 0

; Set the stack pointer to the top of the first bank of RAM        
        ld SP, 0x2000           

; Set the interrupt mode 1
        im 1
        ei                

; Initialize the monotor serial port
        ld A, 0b00101000         ; 9600 baud, 8 bits, no parity, 1 stop bit
        ld IX, SERIAL_MONITOR
        call init_serial

; check hardware flags for using monitor during startup
        in(HARDWARE_FLAGS), A
        and 0x01;
        jr Z, startup2

; Print system startup message
        ld HL, str_version
        call serial_write

startup2:
        call watchdog_kick
; Check non system ram memory
        in(HARDWARE_FLAGS), A
        and 0x02;
        jr Z, startup3

; RAM should be consequitive from 0x2000 to 0x7FFF (6 banks)
        ld HL, 0x2000
        call check_ram
        cp A, 0
        jr Z, show_ram
        ld H, 0x30
        call check_ram
        cp A, 0
        jr Z, show_ram
        ld H, 0x40
        call check_ram
        cp A, 0
        jr Z, show_ram
        ld H, 0x50
        call check_ram
        cp A, 0
        jr Z, show_ram
        ld H, 0x60
        call check_ram
        cp A, 0
        jr Z, show_ram
        ld H, 0x70
        call check_ram
        cp A, 0
        jr Z, show_ram
; Set H regfister to 0x80 to indicate 6 banks of RAM
        ld H, 0x80
show_ram:
; HL register contains the last bank of memory 
; the H register contains the high byte of the bank.

        ld A, H
        rra                     ; Shift right 4 bits to get the number of ram banks
        rra
        rra
        rra
        dec A                   ; Subtract 2 to get the number of ram banks
        and 0x07                ; Clear the upper 5 bits
        ld B, A                 ; Save the number of banks
        in A, (RAM_STATUS)      ; Set RAM status register
        and 0xF8                ; Clear the lower 3 bits
        or B                    ; Set the number of banks
        out(RAM_STATUS), A  

; check hardware flags for using monitor during startup
        in A, (HARDWARE_FLAGS)
        and 0x01;
        jr Z, startup3

; Print system RAM message
        push A;                 ; Save the number of banks
        ld HL, str_system_ram
        call serial_write

; Print the number of bytes of RAM
        pop A;                  ; Restore the number of banks
        rla                     ; Shift left 4 bits to get the number of bytes
        rla
        rla
        rla

        ; TODO - Print the number of bytes of RAM
        
        ld HL, str_bytes
        call serial_write

startup3:
        call watchdog_kick      
; End boottstrap code, start main code
        jp MAIN

interrupt:

; Watchdog routine
watchdog_routine:
        in(WATCHDOG), A
        cp 0
        jr NZ, watchdog_halt
        retn
watchdog_halt:        
        ld IX, watchdog_routine
        jp system_halt

; Reset the watchdog timer
watchdog_kick:
        ld A, 0
        out (WATCHDOG), 0
        ret

; Halt the system
system_halt:


        halt
        jp system_halt

    ; Verify a bank of memory
    ; using the HL register as a base address
    ; and the DE as counter
check_ram:
        ld DE, 0x1000
check_ram_loop:
    ; Read a byte from memory
        ld A, (HL)
        ld B, A
    ; Set the byte to 0x55 and test it
        ld A, 0x55
        ld (HL), A
        ld A, (HL)
        cp 0x55
        jr NZ, check_ram_error
    ; Write the byte back to memory
        ld A, B
        ld (HL), A
        inc HL
        dec BC
        jr NZ, check_ram_loop
    ; Return with success (zero)
        ld A, 0
        ret
check_ram_error:
    ; Return with error (non-zero)
        ld A, 1
        ld IY, check_ram_error
        ret

; initialize serial port 
; 0b00101000 => 9600 baud, 8 bits, no parity, 1 stop bit
; baud rate  = bits 0-1 of the control register
; stop bits  = bit 2 of the control register
; parity bit = bit 3 of the control register
; data bits  = bits 4-5 of the control register
; status bits= bits 6-7 of the control register 
init_serial:
        out(IX+1), A
        jr wait_for_port
        ret

; Write a string to the serial port in HL
serial_write:
        ld A, (HL)
        cp 0
        jr Z, serial_write_done
serial_write_loop:
        call wait_for_port
        out(IX), A
        inc HL
        ld A, (HL)
        cp 0
        jr NZ, serial_write_loop
serial_write_done:        
        ret 

; Write A single byte to the serial port in HL
serial_write_byte:                            
        ld A, (HL)
        out(IX), A
        ret
; Wait for the serial port to contain data        
wait_for_data:
        in A, (IX+1)
        and 0x01                    ; Check if data is available bit
        jr Z, wait_for_data
        in A, (IX)
        ret
; Wait for the serial port to be ready for transmission        
wait_for_port:
        in A, (IX+1)
        and 0x02                    ; Check if port is available bit
        jr Z, wait_for_port
        ret

system_messages:
str_version:
    DB "ZC-80 System v1.0", 0
str_system_ram:
    DB "System RAM:", 0
str_bytes:    
    DB "bytes", 0
str_error:
    DB "ERR:", 0
str_warn:
    DB "WARN:", 0

